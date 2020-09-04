public class Ray
{
  public Ray(PVector start, PVector dir, int maxBounces)
  {
    this.m_start = start;
    this.m_diriction = dir.normalize();

    m_normal = new PVector(-dir.y, dir.x);
    m_normal.normalize();

    m_bounces = new ArrayList<Ray>();
    m_lineSegments = new ArrayList<LineSegment>();
    m_maxBounces = maxBounces;
    
    m_color = new Color(1,1,1);
  }

  public void set(PVector start, PVector dir)
  {
    this.m_start = start;
    this.m_diriction = dir;

    m_normal = new PVector(-dir.y, dir.x);
    m_normal.normalize();
  }
  
  public void setColor(Color cl)
  {
    m_color = cl;
  }

  public void render()
  {
    // This function is also used as a reset for which bounces have been found
    // 
    for (int i = 0; i < m_bounces.size(); ++i)
    {
      m_bounces.get(i).render();
    }
    m_bounces.clear();
    m_lineSegments.clear();
    
    stroke(m_color.get());

    if (m_hasEnd) line(m_start.x, m_start.y, m_end.x, m_end.y);
    else line(m_start.x, m_start.y, m_start.x+m_diriction.x*500, m_start.y+m_diriction.y*500);
    m_hasEnd = false;
  }
  
  public void renderNormal()
  {
    line(m_start.x, m_start.y, m_start.x + vscale(m_normal, 10).x, m_start.y + vscale(m_normal, 10).y);
  }

  public boolean collideWith(LineSegment other)
  {
    m_lineSegments.add(other);
    for (int i = 0; i < m_bounces.size(); ++i)
    {
      m_bounces.get(i).collideWith(other);
    }

    PVector startToStart = new PVector(other.end().x, other.end().y);
    startToStart.sub(m_start);

    //project distance line on normal
    float distToLine = startToStart.dot(m_normal);
    PVector line = vsub(other.m_start, other.m_end);
    float totalLine = -line.dot(m_normal);

    float timeOfImpact = 1 - distToLine / totalLine;
    if (timeOfImpact < 0 || timeOfImpact > 1)
    {
      
      startToStart = new PVector(other.end().x, other.end().y);
      startToStart.sub(m_start);

      //project distance line on normal
      distToLine = startToStart.dot(m_normal);
      line = vsub(other.m_start, other.m_end);
      totalLine = -line.dot(m_normal);

      timeOfImpact = 1 - distToLine / totalLine;
      
      if (timeOfImpact < 0 || timeOfImpact > 1)
      {
        println(millis() + ": Returned false @ time of impact");
        return false;
      }
    }

    PVector otherStartToPoi = vscale(vsub(other.m_end, other.m_start), timeOfImpact);
    PVector poi = vadd(other.m_start, otherStartToPoi);

    if (vlength(otherStartToPoi) > vlength(vsub(other.m_end, other.m_start)))
    {
      //println("poi is over the end of line");
      return false;
    }

    //this can also be seen as the full line from here
    PVector thisToPoi = vsub(poi, m_start);
    if (thisToPoi.dot(vnormalized(m_diriction)) < 0)
    {
      //println("poi was before the start of the ray");
      return false;
    }

    float poiDistanceOverOther = thisToPoi.dot(vnormalized(line));
    if (poiDistanceOverOther < 0)
    {
      //println("poi was under the start of the line: " + poiDistanceOverOther);
      return false;
    } else
    {
      if (m_hasEnd)
      {
        if (vlength(vsub(m_end, m_start)) < vlength(vsub(poi, m_start)))
        {
          return true;
        }
      }
      Color mixColor = m_color.clone().mult(other.getColor());
      fill(mixColor.get());
      ellipse(poi.x, poi.y, 10, 10);
      m_end = poi;
      m_hasEnd = true;

      if (m_maxBounces != 0)
      {
        //make a new ray as a bounce to this one
        PVector reflectDir = vnormalized(vreflect(m_diriction, other.normal()));
        
        Ray reflect = new Ray(vadd(poi, vscale(reflectDir, 5)), reflectDir, m_maxBounces-1);
        reflect.setColor( mixColor );
        m_bounces.add(reflect);
        for (int i = 0; i < m_lineSegments.size(); ++i)
        {
          reflect.collideWith(m_lineSegments.get(i));
        }
      }

      return true;
    }
  }

  public PVector start()
  {
    return new PVector(m_start.x, m_start.y);
  }

  public PVector dir()
  {
    return new PVector(m_diriction.x, m_diriction.y);
  }

  public PVector normal()
  {
    return new PVector(m_normal.x, m_normal.y);
  }

  private PVector m_start;
  private PVector m_diriction;
  private PVector m_end;
  private boolean m_hasEnd = false;
  private PVector m_normal;
  private int m_maxBounces;
  private Color m_color;

  private ArrayList<Ray> m_bounces;
  private ArrayList<LineSegment> m_lineSegments;
}
