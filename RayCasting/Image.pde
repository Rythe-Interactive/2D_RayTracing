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

  // Generates collisions for the image based on alpha channel
  public void init()
  {
    ArrayList<LineSegment> lines = new ArrayList<LineSegment>();
    

    //first find the top left corner
    PVector topLeft = findTopLeft();

    println("Top left: " + topLeft);
    if(topLeft == null) return;
    // found top left
    
    ArrayList<PVector> linePoints = new ArrayList<PVector>();
    linePoints.add(topLeft);
    
    getLinePointsTop(topLeft, linePoints);
    getLinePointsRight(linePoints.get(linePoints.size()-1), linePoints);
    getLinePointsBottom(linePoints.get(linePoints.size()-1), linePoints);
    getLinePointsLeft(linePoints.get(linePoints.size()-1), linePoints);
    
    //for(int i = 0; i < topPoints.size(); ++i)
    //{
    //  linePoints.add(topPoints.get(i));
    //}
    
    for(int i = 0; i < linePoints.size(); ++i)
    {
      PVector addToPixelPosisitions = vsub(m_position, new PVector(m_image.width/2, m_image.height / 2));
      LineSegment line;
      if(i+1 == linePoints.size())
      {
        line = new LineSegment(vadd(linePoints.get(i), addToPixelPosisitions), vadd(linePoints.get(0), addToPixelPosisitions));
      }
      else
      {
        line = new LineSegment(vadd(linePoints.get(i), addToPixelPosisitions), vadd(linePoints.get(i+1), addToPixelPosisitions));
      }
      lines.add(line);
    }

    m_lineSegments = new LineSegment[lines.size()];
    m_colliderCount = lines.size();
    println(m_colliderCount);
    for (int i = 0; i < lines.size(); ++i)
    {
      println("    [" + i + "]: " + lines.get(i));
      m_lineSegments[i] = lines.get(i);
    }
  }
  
  private PVector findTopLeft()
  {
    PVector topLeft = null;

    for (int i = 0; i < max(m_image.width, m_image.height); ++i)
    {
      for (int y = 0; y <= i; ++y)
      {
        for (int x = 0; x <= i; ++x)
        {
          if(y < i && x < i) continue;
          Color pixel = new Color(m_image.pixels[x+y*m_image.width]);
          if (pixel.a == 1)
          {
            topLeft = new PVector(x, y);
            break;
          }
        }
        // end of x < width loop
        if(topLeft != null) break;
      }
      // end of y < height loop
       if(topLeft != null) break;
    }
    // end of i < width*height loop
    
    return topLeft;
  }
  
  private void getLinePointsTop(PVector start, ArrayList<PVector> linePoints)
  {
    PVector lastPixelInSequence = null;
    PVector sequenceStep = null;
    PVector startPixelInSequence = null;
    //search to the right
    for(int x = (int)start.x; x < m_image.width; ++x)
    {
      boolean foundPixel = false;
      for(int y = 0; y < m_image.height; ++y)
      {
        Color cl = new Color(m_image.pixels[x+y*m_image.width]);
        if(cl.a == 1)
        {
          foundPixel = true;
          PVector pixel = new PVector(x, y);
          if(vcompare(pixel, linePoints.get(0)) && startPixelInSequence != null) return;
          if(startPixelInSequence == null)
          {
            println("pixel " + pixel + " start of sequence");
            startPixelInSequence = pixel;
            lastPixelInSequence = pixel;
            break;
          }
          PVector step = vsub(pixel, lastPixelInSequence);
          if(sequenceStep == null || vcompare(step, sequenceStep))
          {
            //same step as last step - no need for a new lineSegment
            println("pixel " + pixel + " continue of sequence");
            println("        " + step + " vs " + sequenceStep);
            lastPixelInSequence = pixel;
            sequenceStep = step;
            break;
          }
          else if(!vcompare(step, sequenceStep))
          {
            // new pixel is next to last pixel
            println("pixel " + pixel + " exit of sequence");
            println("        " + step + " vs " + sequenceStep);
            linePoints.add(lastPixelInSequence);
            lastPixelInSequence = null;
            startPixelInSequence = pixel;
            sequenceStep = step;
          }
          lastPixelInSequence = pixel;
          break;
        }
      }
      if(lastPixelInSequence != null && !foundPixel)
      {
        println("Gab found");
        linePoints.add(lastPixelInSequence);
        lastPixelInSequence = null;
      }
    }
  }
  
  
  private void getLinePointsRight(PVector start, ArrayList<PVector> linePoints)
  {
    PVector lastPixelInSequence = null;
    PVector sequenceStep = null;
    PVector startPixelInSequence = null;
    //search to the right
    for(int y = (int)start.y; y < m_image.height; ++y)
    {
      boolean foundPixel = false;
      for(int x = m_image.height - 1; x >= 0; --x)
      {
        Color cl = new Color(m_image.pixels[x+y*m_image.width]);
        if(cl.a == 1)
        {
          foundPixel = true;
          PVector pixel = new PVector(x, y);
          if(vcompare(pixel, linePoints.get(0)) && startPixelInSequence != null) return;
          if(startPixelInSequence == null)
          {
            println("pixel " + pixel + " start of sequence");
            startPixelInSequence = pixel;
            lastPixelInSequence = pixel;
            break;
          }
          PVector step = vsub(pixel, lastPixelInSequence);
          if(sequenceStep == null || vcompare(step, sequenceStep))
          {
            //same step as last step - no need for a new lineSegment
            println("pixel " + pixel + " continue of sequence");
            println("        " + step + " vs " + sequenceStep);
            lastPixelInSequence = pixel;
            sequenceStep = step;
            break;
          }
          else if(!vcompare(step, sequenceStep))
          {
            // new pixel is next to last pixel
            println("pixel " + pixel + " exit of sequence");
            println("        " + step + " vs " + sequenceStep);
            linePoints.add(lastPixelInSequence);
            lastPixelInSequence = null;
            startPixelInSequence = pixel;
            sequenceStep = step;
          }
          lastPixelInSequence = pixel;
          break;
        }
      }
      if(lastPixelInSequence != null && !foundPixel)
      {
        println("Gab found");
        linePoints.add(lastPixelInSequence);
        lastPixelInSequence = null;
      }
    }
  }
  
  private void getLinePointsBottom(PVector start, ArrayList<PVector> linePoints)
  {
    PVector lastPixelInSequence = null;
    PVector sequenceStep = null;
    PVector startPixelInSequence = null;
    //search to the right
    for(int x = (int)start.x; x >= 0; --x)
    {
      boolean foundPixel = false;
      for(int y = m_image.height-1; y >= 0; --y)
      {
        Color cl = new Color(m_image.pixels[x+y*m_image.width]);
        if(cl.a == 1)
        {
          foundPixel = true;
          PVector pixel = new PVector(x, y);
          if(vcompare(pixel, linePoints.get(0)) && startPixelInSequence != null) return;
          if(startPixelInSequence == null)
          {
            println("pixel " + pixel + " start of sequence");
            startPixelInSequence = pixel;
            lastPixelInSequence = pixel;
            break;
          }
          PVector step = vsub(pixel, lastPixelInSequence);
          if(sequenceStep == null || vcompare(step, sequenceStep))
          {
            //same step as last step - no need for a new lineSegment
            println("pixel " + pixel + " continue of sequence");
            println("        " + step + " vs " + sequenceStep);
            lastPixelInSequence = pixel;
            sequenceStep = step;
            break;
          }
          else if(!vcompare(step, sequenceStep))
          {
            // new pixel is next to last pixel
            println("pixel " + pixel + " exit of sequence");
            println("        " + step + " vs " + sequenceStep);
            linePoints.add(lastPixelInSequence);
            lastPixelInSequence = null;
            startPixelInSequence = pixel;
            sequenceStep = step;
          }
          lastPixelInSequence = pixel;
          break;
        }
      }
      if(lastPixelInSequence != null && !foundPixel)
      {
        println("Gab found");
        linePoints.add(lastPixelInSequence);
        lastPixelInSequence = null;
      }
    }
  }
  
  private void getLinePointsLeft(PVector start, ArrayList<PVector> linePoints)
  {
    PVector lastPixelInSequence = null;
    PVector sequenceStep = null;
    PVector startPixelInSequence = null;
    //search to the right
    for(int y = (int)start.y; y >= 0; --y)
    {
      boolean foundPixel = false;
      for(int x = 0; x < m_image.width; ++x)
      {
        Color cl = new Color(m_image.pixels[x+y*m_image.width]);
        if(cl.a == 1)
        {
          foundPixel = true;
          PVector pixel = new PVector(x, y);
          if(vcompare(pixel, linePoints.get(0)) && startPixelInSequence != null) return;
          if(startPixelInSequence == null)
          {
            println("pixel " + pixel + " start of sequence");
            startPixelInSequence = pixel;
            lastPixelInSequence = pixel;
            break;
          }
          PVector step = vsub(pixel, lastPixelInSequence);
          if(sequenceStep == null || vcompare(step, sequenceStep))
          {
            //same step as last step - no need for a new lineSegment
            println("pixel " + pixel + " continue of sequence");
            println("        " + step + " vs " + sequenceStep);
            lastPixelInSequence = pixel;
            sequenceStep = step;
            break;
          }
          else if(!vcompare(step, sequenceStep))
          {
            // new pixel is next to last pixel
            println("pixel " + pixel + " exit of sequence");
            println("        " + step + " vs " + sequenceStep);
            linePoints.add(lastPixelInSequence);
            lastPixelInSequence = null;
            startPixelInSequence = pixel;
            sequenceStep = step;
          }
          lastPixelInSequence = pixel;
          break;
        }
      }
      if(lastPixelInSequence != null && !foundPixel)
      {
        println("Gab found");
        linePoints.add(lastPixelInSequence);
        lastPixelInSequence = null;
      }
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
    for (int i = 0; i < m_colliderCount; ++i)
    {
      m_lineSegments[i].render();
    }
  }

  private PImage m_image;
  private PVector m_position;
  private LineSegment[] m_lineSegments;
  private int m_colliderCount = 0;
}
