public class LineSegment
{
  public LineSegment(PVector start, PVector end)
  {
    this.m_start = start;
    this.m_end = end;
    
    PVector line = new PVector(start.x, start.y);
    line.sub(end);
    m_normal = new PVector(-line.y, line.x);
    m_normal.normalize();
    m_color = new Color(1,1,1);
  }
  
  public void set(PVector start, PVector end)
  {
    this.m_start = start;
    this.m_end = end;
    
    PVector line = new PVector(start.x, start.y);
    line.sub(end);
    m_normal = new PVector(-line.y, line.x);
    m_normal.normalize();
  }
  
  public void setColor(Color cl)
  {
    m_color = cl;
  }
  
  public Color getColor()
  {
    return m_color;
  }
  
  public void render()
  {
    stroke(m_color.get());
    line(m_start.x, m_start.y, m_end.x, m_end.y);
  }
  
  public void renderNormal()
  {
    PVector line = new PVector(m_end.x, m_end.y);
    line.sub(m_start);
    line = line.mult(0.5f);
    PVector point = new PVector(m_start.x, m_start.y);
    point.add(line);
    line(point.x, point.y, point.x + m_normal.x * 20, point.y + m_normal.y * 20);
  }
  
  public boolean collideWith(LineSegment other)
  {
    PVector startToStart = new PVector(other.m_end.x, other.m_end.y);
    startToStart.sub(m_start);
    
    //project distance line on normal
    float distToLine = startToStart.dot(m_normal);
    PVector line = vsub(other.m_start, other.m_end);
    float totalLine = -line.dot(m_normal);
    
    float timeOfImpact = 1 - distToLine / totalLine;
    if(timeOfImpact < 0 || timeOfImpact > 1) return false;
    
    PVector vec = vscale(vsub(other.m_start, other.m_end), timeOfImpact);
    PVector poi = vsub(other.m_start, vec);
    PVector toPoi = vsub(poi, m_start);
    float distToPoi = toPoi.dot(vnormalized(vsub(m_end, m_start)));
    println(distToPoi);
    if(distToPoi < 0 || distToPoi > vlength(vsub(m_end, m_start))) return false;
    else
    {
      fill(255, 0 ,0);
      ellipse(poi.x, poi.y, 10, 10);
      return true;
    }
  }
  
  public boolean collideWith(PVector point, float radius)
  {
    PVector startToCenter = new PVector(point.x, point.y);
    startToCenter.sub(m_start);
    
    PVector line = vsub(m_end, m_start);
    float distOverLine = startToCenter.dot(vnormalized(line));
    if(distOverLine < -radius || distOverLine > vlength(line)+radius) return false;
    
    //project distance line on normal
    float dist = startToCenter.dot(m_normal);
    if(abs(dist) < radius) return true;
    else return false;
  }
  
  public PVector start()
  {
    return new PVector(m_start.x, m_start.y);
  }
  
  public PVector end()
  {
    return new PVector(m_end.x, m_end.y);
  }
  
  public PVector getLine()
  {
    return vsub(m_end, m_start);
  }
  
  public PVector getNormal()
  {
    return new PVector(m_normal.x, m_normal.y);
  }
  
  public LineSegment inverse()
  {
    LineSegment inverse = new LineSegment(m_end, m_start);
    inverse.setColor(m_color);
    return inverse;
  }
  
  public String toString()
  {
    return m_start + " to " + m_end;
  }
  
  private PVector m_start;
  private PVector m_end;
  private PVector m_normal;
  private Color m_color;
}
