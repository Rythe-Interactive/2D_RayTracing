class Slider
{
  public Slider(float posX, float posY, int w, int h)
  {
    m_position = new PVector(posX, posY);
    m_size = new PVector(w, h);
    m_color = color(255, 255, 255);
    m_knobColor = color(120, 120, 120);
  }

  public void update()
  {
    PVector knobPos = getKnobPosition();
    PVector knobSize = getKnobSize();
    if (mousePressed)
    {
      if ( mouseX >= knobPos.x - knobSize.x/2 && mouseX <= knobPos.x + knobSize.x/2 && mouseY >= knobPos.y - knobSize.y/2 && mouseY <= knobPos.y + knobSize.y/2)
      {
        m_holdingSlider = true;
      }
    } 
    else m_holdingSlider = false;

    if (m_holdingSlider)
    {
      float toBottom = (m_position.y + m_size.y/2) - mouseY;
      m_value = toBottom / (m_size.y);
      m_value = constrain(m_value, 0, 1);
    }
  }

  public void render()
  {
    rectMode(CENTER);
    fill(m_color);
    stroke(0);
    rect(m_position.x, m_position.y, m_size.x, m_size.y);
    fill(m_knobColor);
    PVector knobPos = getKnobPosition();
    PVector knobSize = getKnobSize();
    rect(knobPos.x, knobPos.y, knobSize.x, knobSize.y);
  }
  
  public void renderText()
  {
    textAlign(CENTER);
    fill(0);
    text(m_value, m_position.x, m_position.y);
  }

  public float getValue()
  {
    return m_value;
  }
  
  public boolean isChanging()
  {
    return m_holdingSlider;
  }
  
  public void setColor(color cl)
  {
    m_color = cl;
  }
  
  public void setKnoColor(color cl)
  {
    m_knobColor = cl;
  }

  public void setValue(float value)
  {
    m_value = value;
  }

  private PVector getKnobPosition()
  {
    return new PVector(m_position.x, m_position.y-(m_value - 0.5f)*m_size.y);
  }

  private PVector getKnobSize()
  {
    return new PVector( m_size.x*1.3f, m_size.y/10);
  }

  private float m_value;
  private PVector m_size;
  private PVector m_position;
  private boolean m_holdingSlider = false;
  private color m_color;
  private color m_knobColor;
}
