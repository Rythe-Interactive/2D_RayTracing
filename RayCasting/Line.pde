public class Line
{
  public Line(PVector start, PVector dir)
  {
    this.m_start = start;
    this.m_dir = dir.normalize();
    
    m_normal = new PVector(-dir.y, dir.x);
    m_normal.normalize();
  }
  
  public void set(PVector start, PVector dir)
  {
    this.m_start = start;
    this.m_dir = dir;
    
    m_normal = new PVector(-dir.y, dir.x);
    m_normal.normalize();
  }
  
  public void render()
  {
    line(m_start.x, m_start.y, m_start.x+m_dir.x*500, m_start.y+m_dir.y*500);
  }
  
  public boolean collideWith(LineSegment other)
  {
    PVector startToStart = new PVector(other.end().x, other.end().y);
    startToStart.sub(m_start);
    
    //project distance line on normal
    float distToLine = startToStart.dot(m_normal);
    PVector line = vsub(other.m_start, other.m_end);
    float totalLine = -line.dot(m_normal);
    
    float timeOfImpact = 1 - distToLine / totalLine;
    if(timeOfImpact < 0 || timeOfImpact > 1) return false;
    
    PVector vec = vscale(vsub(other.m_start, other.m_end), timeOfImpact);
    PVector poi = vsub(other.m_start, vec);
    
    //we may assume that the 'end' of the line is at the poi
    PVector toPoi = vsub(poi, m_start);
    
    //check if poi is behind the line (before the start)
    if(vnormalized(toPoi).dot(vnormalized(m_dir)) < 0)
    {
      return false;
    }
    
    float distToPoi = toPoi.dot(vnormalized(vsub(poi, m_start)));
    if(distToPoi < 0 || distToPoi > vlength(vsub(poi, m_start)))
    {
      println("out of line");
      return false;
    }
    else
    {
      fill(0, 0, 255);
      ellipse(poi.x, poi.y, 10, 10);
      return true;
    }
  }
  
  public PVector start()
  {
    return new PVector(m_start.x, m_start.y);
  }
  
  public PVector dir()
  {
    return new PVector(m_dir.x, m_dir.y);
  }
  
  public PVector normal()
  {
    return new PVector(m_normal.x, m_normal.y);
  }
  
  private PVector m_start;
  private PVector m_dir;
  private PVector m_normal;
}
