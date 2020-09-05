abstract public class Light
{
  public Light()
  {
    m_rays = new ArrayList<Ray>();  
  }
  
  abstract public void setRayCount(int rays);
  
  public void addLineSegmentCollider(LineSegment line)
  {
    for(int i = 0; i < m_rays.size(); ++i)
    {
      m_rays.get(i).addLineSegmentCollider(line);
    }
  }
  
  public void render()
  {
    for(int i = 0; i < m_rays.size(); ++i)
    {
      m_rays.get(i).render();
    }
  }
  
  public void update()
  {
    for(int i = 0; i < m_rays.size(); ++i)
    {
      m_rays.get(i).update();
    }
  }
  
  protected ArrayList<Ray> m_rays;
}
