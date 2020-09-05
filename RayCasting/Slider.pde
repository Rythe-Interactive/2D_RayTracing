class Slider
{
  public Slider(float posX, float posY, int w, int h)
  {
    m_position = new PVector(posX, posY);
    m_size = new PVector(w, h);
  }

  public void update()
  {
    PVector knobPos = getKnobPosition();
    PVector knobSize = getKnobSize();
    if (mousePressed)
    {
      if ( mouseX >= knobPos.x - knobSize.x && mouseX <= knobPos.x + knobSize.x && mouseY >= knobPos.y - knobSize.y && mouseY <= knobPos.y + knobSize.y)
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
    fill(255);
    stroke(0);
    rect(m_position.x, m_position.y, m_size.x, m_size.y);
    fill(0, 0, 255);
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
}
