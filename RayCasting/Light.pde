abstract public class Light
{
  public Light()
  {
    m_color = new Color(1,1,1);
  }
  
  public Light(int rayCount)
  {
    m_color = new Color(1,1,1);
    setRayCount(rayCount);
  }
  
  abstract public void setRayCount(int rayCount);
  public void setPosition(PVector pos)
  {
    m_position = pos;
  }
  
  public void setColor(Color clr)
  {
    m_color = clr;
    for (int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].setColor(m_color);
    }
  }
  
  public void addLineSegmentCollider(LineSegment line)
  {
    for(int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].addLineSegmentCollider(line);
    }
  }
  
  // Renders all the rays of the light
  public void renderRays()
  {
    for(int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].render();
    }
  }
  
  // Renders only where the light is in the shape of a light
  // Does not render the rays
  public void renderPosition()
  {
    fill(120);
    arc(m_position.x, m_position.y, 10, 40, degToRad(0), degToRad(180));
    fill(m_color.get());
    ellipse(m_position.x, m_position.y, 20, 20);
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
  protected Color m_color;
  protected PVector m_position;
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
    m_position = position;
    for (int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].set(vadd(new PVector(mouseX, mouseY), vscale(m_rays[i].dir(), 10)), m_rays[i].dir());
    }
  }
}
