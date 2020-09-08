abstract class Scene
{
  abstract public void init();
  abstract public void update();
  abstract public void handleInput();
}

// ---------------------------------------------- Start of Hexagon Scene --------------------------------------------------

class HexagonScene extends Scene
{
  public void init()
  {    
    m_leftTopLine = new LineSegment(new PVector(101, 51), new PVector(49, 250));
    m_leftTopLine.setColor(new Color(0.9, 0.5, 0));

    m_leftBottomLine = new LineSegment(new PVector(100, 450), new PVector(50, 250));
    m_leftBottomLine.setColor(new Color(0.2, 0.9, 0.1));

    m_rightTopLine = new LineSegment(new PVector(400, 50), new PVector(450, 250));
    m_rightTopLine.setColor(new Color(0, 0.5, 0.9));

    m_rightBottomLine = new LineSegment(new PVector(400, 450), new PVector(450, 250));
    m_rightBottomLine.setColor(new Color(0.9, 0.9, 0.9));

    m_topLine = new LineSegment(new PVector(100, 50), new PVector(400, 50));
    m_topLine.setColor(new Color(1, 1, 0));

    m_bottomLine = new LineSegment(new PVector(100, 450), new PVector(400, 450));
    m_bottomLine.setColor(new Color(1, 0, 1));

    m_rays = new Ray[m_rayCount];
    for (int i = 0; i < m_rayCount; ++i)
    {
      PVector dir = PVector.fromAngle(PI*2/m_rayCount*i);
      m_rays[i] = new Ray(new PVector(0, 0), dir, m_maxBounces);

      m_rays[i].addLineSegmentCollider(m_topLine);
      m_rays[i].addLineSegmentCollider(m_rightTopLine);
      m_rays[i].addLineSegmentCollider(m_leftTopLine);
      m_rays[i].addLineSegmentCollider(m_leftBottomLine);
      m_rays[i].addLineSegmentCollider(m_rightBottomLine);
      m_rays[i].addLineSegmentCollider(m_bottomLine);
    }
  }

  public void update()
  {
    background(0);

    PVector mouseDir = new PVector(mouseX-250, mouseY-250);
    mouseDir.normalize();

    for (int i = 0; i < m_rayCount; ++i)
    {
      m_rays[i].set(vadd(new PVector(mouseX, mouseY), vscale(m_rays[i].dir(), 10)), m_rays[i].dir());
      m_rays[i].update();

      m_rays[i].render();
    }

    m_topLine.render();
    m_leftTopLine.render();
    m_leftBottomLine.render();
    m_rightTopLine.render();
    m_rightBottomLine.render();
    m_bottomLine.render();

    fill(255);
    stroke(255);
    textSize(12);
    text(frameRate, 30, 10);
    text("Max bounces per ray: " + m_maxBounces, width-75, 10);
    text("Ray Count: " + m_rayCount + "              ", width-75, 20);

    text("W: add ray  D: remove ray", width /2, height - 20);
    text("A: increase bounce count  S: decrease bounce count", width / 2, height - 10);
  }

  public void handleInput()
  {
    if (key == 'a')
    {
      --m_maxBounces;
      for (int i = 0; i < m_rayCount; ++i)
      {
        m_rays[i].setMaxBounces(m_maxBounces);
      }
    }
    if (key == 'd')
    {
      ++m_maxBounces;
      for (int i = 0; i < m_rayCount; ++i)
      {
        m_rays[i].setMaxBounces(m_maxBounces);
      }
    }
    if (key == 'w')
    {
      ++m_rayCount;
      m_rays = new Ray[m_rayCount];
      for (int i = 0; i < m_rayCount; ++i)
      {
        PVector dir = PVector.fromAngle(PI*2/m_rayCount*i);
        m_rays[i] = new Ray(new PVector(0, 0), dir, m_maxBounces);

        m_rays[i].addLineSegmentCollider(m_topLine);
        m_rays[i].addLineSegmentCollider(m_rightTopLine);
        m_rays[i].addLineSegmentCollider(m_leftTopLine);
        m_rays[i].addLineSegmentCollider(m_leftBottomLine);
        m_rays[i].addLineSegmentCollider(m_rightBottomLine);
        m_rays[i].addLineSegmentCollider(m_bottomLine);
      }
    }
    if (key == 's')
    {
      --m_rayCount;
      m_rays = new Ray[m_rayCount];
      for (int i = 0; i < m_rayCount; ++i)
      {
        PVector dir = PVector.fromAngle(PI*2/m_rayCount*i);
        m_rays[i] = new Ray(new PVector(0, 0), dir, m_maxBounces);

        m_rays[i].addLineSegmentCollider(m_topLine);
        m_rays[i].addLineSegmentCollider(m_rightTopLine);
        m_rays[i].addLineSegmentCollider(m_leftTopLine);
        m_rays[i].addLineSegmentCollider(m_leftBottomLine);
        m_rays[i].addLineSegmentCollider(m_rightBottomLine);
        m_rays[i].addLineSegmentCollider(m_bottomLine);
      }
    }
  }

  private LineSegment m_leftTopLine;
  private LineSegment m_leftBottomLine;
  private LineSegment m_rightTopLine;
  private LineSegment m_rightBottomLine;
  private LineSegment m_topLine;
  private LineSegment m_bottomLine;

  private Ray[] m_rays;
  private int m_rayCount = 30;
  private int m_maxBounces = 4;
}

// ---------------------------------------------- End of Hexagon Scene --------------------------------------------------

// ---------------------------------------------- Start of SandBox Scene --------------------------------------------------

class SandBoxScene extends Scene
{
  public void init()
  {
    m_lights = new ArrayList<Light>();
    m_lineSegments = new ArrayList<LineSegment>();
    
    
    m_colorSliders = new Slider[3];
    m_colorSliders[0] = new Slider(width-100, 40, 20, 50);
    m_colorSliders[0].setValue(1);
    m_colorSliders[1] = new Slider(width-60, 40, 20, 50);
    m_colorSliders[1].setValue(1);
    m_colorSliders[2] = new Slider(width-20, 40, 20, 50);
    m_colorSliders[2].setValue(1);
    
    for(int i = 0; i < 3; ++i)
    {
      m_colorSliders[i].setKnoColor(color(255));
    }
  }

  public void update()
  {
    background(0);

    boolean changingSlider = false;

    fill(getColorFromSliders().get());
    rect(width-60, 80, 100, 10);
    for(int i = 0; i < 3; ++i)
    {
      m_colorSliders[i].update();
      if(m_colorSliders[i].isChanging()) changingSlider = true;
      m_colorSliders[i].render();
    }
    
    m_colorSliders[0].setColor(color(m_colorSliders[0].getValue()*255, 0, 0));
    m_colorSliders[1].setColor(color(0, m_colorSliders[1].getValue()*255, 0));
    m_colorSliders[2].setColor(color(0, 0, m_colorSliders[2].getValue()*255));

    if (mousePressed && !changingSlider)
    {
      if (!m_pressingMouse)
      {
        m_pressingMouse = true;
        // left mouse
        if (mouseButton == LEFT)
        {
          if (m_startOfNextLine == null)
          {
            m_startOfNextLine = new PVector(mouseX, mouseY);
          } else
          {
            PVector end = new PVector(mouseX, mouseY);
            LineSegment line = new LineSegment(m_startOfNextLine, end);
            line.setColor(getColorFromSliders());
            m_startOfNextLine = null;
            for (int i = 0; i < m_lights.size(); ++i)
            {
              m_lights.get(i).addLineSegmentCollider(line);
            }
            m_lineSegments.add(line);
          }
        }
        // End of left mouse
        // Right mouse
        else if (mouseButton == RIGHT)
        {
          PointLight light = new PointLight(20);
          light.setPosition(new PVector(mouseX, mouseY));
          light.setColor(getColorFromSliders());
          m_lights.add(light);
          for (int i = 0; i < m_lineSegments.size(); ++i)
          {
            light.addLineSegmentCollider(m_lineSegments.get(i));
          }
        }
      }
    } else
    {
      m_pressingMouse = false;
    }

    for (int i = 0; i < m_lineSegments.size(); ++i)
    {
      m_lineSegments.get(i).render();
    }
    for (int i = 0; i < m_lights.size(); ++i)
    {
      m_lights.get(i).update();
      if(m_renderLights) m_lights.get(i).renderRays();
      m_lights.get(i).renderPosition();
    }
    
    fill(255);
    stroke(255);
    textSize(12);
    text(frameRate, 30, 10);
    text("Left mouse: set start/set end of new line segment", width /2, height - 30);
    text("Right mouse: create new light at mouse position", width / 2, height - 20);
    text("C: clear scene  R: toggle light render", width /2, height - 10);
  }
  
  private Color getColorFromSliders()
  {
    return new Color(m_colorSliders[0].getValue(), m_colorSliders[1].getValue(), m_colorSliders[2].getValue());
  }

  public void handleInput()
  {
    if(key == 'c')
    {
      m_lineSegments.clear();
      m_lights.clear();
    }
    else if(key == 'r')
    {
      m_renderLights = !m_renderLights;
    }
  }

  private ArrayList<LineSegment> m_lineSegments;
  private ArrayList<Light> m_lights;

  private PVector m_startOfNextLine = null;
  private boolean m_pressingMouse = false;
  private boolean m_renderLights = true;
  
  private Slider[] m_colorSliders;
}

// ---------------------------------------------- End of SandBox Scene --------------------------------------------------

// ---------------------------------------------- Start of Image Scene --------------------------------------------------

class ImageScene extends Scene
{
  public void init()
  {
    m_image = new Image("big_colored_circle.png", new PVector(width/2, height/2));
    
    m_image.init();
    
    m_lights = new ArrayList<Light>();
  }
  
  public void update()
  {
     background(0, 0 ,0);
     
    if (mousePressed)
    {
     if (!m_pressingMouse)
     {
        m_pressingMouse = true;
        // Right mouse
        if (mouseButton == RIGHT)
        {
          PointLight light = new PointLight(20);
          light.setPosition(new PVector(mouseX, mouseY));
          light.setColor(new Color(1,1,1));
          m_lights.add(light);
          
          LineSegment[] imageColliders = m_image.getColliders();
          for(int i = 0; i < m_image.getColliderCount(); ++i)
          {
            light.addLineSegmentCollider(imageColliders[i]);
          }
        }
      }
    } else
    {
      m_pressingMouse = false;
    }
     
     m_image.renderImage();
     //m_image.renderColliders();
     
    for (int i = 0; i < m_lights.size(); ++i)
    {
      m_lights.get(i).update();
      //m_lights.get(i).renderRays();
      m_lights.get(i).renderRayBounces();
      m_lights.get(i).renderPosition();
    }
  }
  
  public void handleInput()
  {
  }
  
  private Image m_image;
  private ArrayList<Light> m_lights;
  
  private boolean m_pressingMouse = false;
}

// ---------------------------------------------- End of Image Scene --------------------------------------------------
