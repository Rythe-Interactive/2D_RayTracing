class Image
{
  public Image(PImage image, PVector position)
  {
    m_image = image;
    m_position = position;
  }
  
  public void init()
  {
  }
  
  public LineSegment[] getColliders()
  {
    return m_lineSegments;
  }
  
  public int getColliderCount()
  {
    return m_colliderCount;
  }
  
  public void renderImage()
  {
    imageMode(CENTER);
    image(m_image, m_position.x, m_position.y);
  }
  
  public void renderColliders()
  {
    for(int i = 0; i < m_colliderCount; ++i)
    {
      m_lineSegments[i].render();
    }
  }
  
  private PImage m_image;
  private PVector m_position;
  private LineSegment[] m_lineSegments;
  private int m_colliderCount = 0;
}
