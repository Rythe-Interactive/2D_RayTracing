abstract public class Light
{
  public Light()
  {
  }
  
  public Light(int rayCount)
  {
    setRayCount(rayCount);
  }
  
  abstract public void setRayCount(int rayCount);
  abstract public void setPosition(PVector pos);
  
  public void addLineSegmentCollider(LineSegment line)
  {
    for(int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].addLineSegmentCollider(line);
    }
  }
  
  public void render()
  {
    for(int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].render();
    }
  }
  
  public void update()
  {
    for(int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].update();
    }
  }
  
  protected Ray[] m_rays;
  protected int m_rayCount = 0;
}

class PointLight extends Light
{
  public PointLight()
  {
  }
  
  public PointLight(int rayCount)
  {
    super(rayCount);
  }
  
  public void setRayCount(int count)
  {
    m_rayCount = count;
    m_rays = new Ray[m_rayCount];
    for (int i = 0; i < m_rayCount; ++i)
    {
      PVector dir = PVector.fromAngle(PI*2/m_rayCount*i);
      m_rays[i] = new Ray(vadd(new PVector(width / 2, height / 2), vscale(dir, 10)), dir, 2);
    }
  }
  
  public void setPosition(PVector position)
  {
    for (int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].set(vadd(new PVector(mouseX, mouseY), vscale(m_rays[i].dir(), 10)), m_rays[i].dir());
    }
  }
}
