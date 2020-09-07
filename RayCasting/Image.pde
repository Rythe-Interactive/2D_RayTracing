class Image
{
  public Image(PImage image, PVector position)
  {
    m_image = image;
    m_position = position;
  }
  
  public Image(String path, PVector position)
  {
    m_image = loadImage(path);
    m_position = position;
  }
  
  public void init()
  {
    ArrayList<LineSegment> lines = new ArrayList<LineSegment>();
    PVector currentStartPixel = null;
    int skipNext = 0;
    
    for(int y = 0; y < m_image.height; ++y)
    {
      for(int x = 0; x < m_image.width; ++x)
      {
        if(skipNext > 0)
        {
          --skipNext;
          continue;
        }
        
        // if we got here, skipNext == 0
        
        Color pixel = new Color(m_image.pixels[x+y*m_image.width]);
        if(pixel.a == 0)
        {
          // Assume alpha 0 means no collision 
          continue;
        }
        else
        {
          if(currentStartPixel == null)
          {
            currentStartPixel = new PVector(x, y);
            for(int i = x+1; i < m_image.width; ++i)
            {
              Color next = new Color(m_image.pixels[i+y*m_image.width]);
              if(next.a == 0)
              {
                println("breaking");
                break;
              }
              else ++skipNext;
            }
            PVector lineEnd = new PVector(x+skipNext, y);
            LineSegment line = new LineSegment(vadd(vsub(m_position, new PVector(m_image.width/2, m_image.height/2)), currentStartPixel), vadd(vsub(m_position, new PVector(m_image.width/2, m_image.height/2)), lineEnd));
            line.setColor(new Color(0, 0, 1));
            lines.add(line);
          }
          else
          {
            
          }
        }
      }
    }
    
    m_lineSegments = new LineSegment[lines.size()];
    m_colliderCount = lines.size();
    println(m_colliderCount);
    for(int i = 0; i < lines.size(); ++i)
    {
      m_lineSegments[i] = lines.get(i);
    }
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
