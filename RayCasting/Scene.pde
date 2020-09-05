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
  }

  public void update()
  {
    background(0);

    if (mousePressed)
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
      m_lights.get(i).render();
    }
    
    fill(255);
    stroke(255);
    textSize(12);
    text(frameRate, 30, 10);
    text("Left mouse: set start/set end of new line segment", width /2, height - 30);
    text("Right mouse: create new light at mouse position", width / 2, height - 20);
    text("Press C to clear", width /2, height - 10);
  }

  public void handleInput()
  {
    if(key == 'c')
    {
      m_lineSegments.clear();
      m_lights.clear();
    }
  }

  private ArrayList<LineSegment> m_lineSegments;
  private ArrayList<Light> m_lights;

  private PVector m_startOfNextLine = null;
  private boolean m_pressingMouse = false;
}

// ---------------------------------------------- End of SandBox Scene --------------------------------------------------
