class Color
{
  public Color(float cr, float cg, float cb, float ca)
  {
    r = cr;
    g = cg;
    b = cb;
    a = ca;
  }
  
  public Color(float cr, float cg, float cb)
  {
    r = cr;
    g = cg;
    b = cb;
    a = 1;
  }
  
  public Color(Color cl)
  {
    r = cl.r;
    g = cl.g;
    b = cl.b;
    a = cl.a;
  }
  
  public Color(color cl)
  {
    r = red(cl) / 255;
    g = green(cl) / 255;
    b = blue(cl) / 255;
    a = alpha(cl) / 255;
  }
  
  public void set(float cr, float cg, float cb, float ca)
  {
    r = cr;
    g = cg;
    b = cb;
    a = ca;
  }
  
  public void set(float cr, float cg, float cb)
  {
    r = cr;
    g = cg;
    b = cb;
    a = 1;
  }
  
  public void set(color cl)
  {
    r = red(cl) / 255;
    g = green(cl) / 255;
    b = blue(cl) / 255;
    a = alpha(cl) / 255;
  }
  
  public Color fromColor(color cl)
  {
    Color clr = new Color(cl);
    return clr;
  }
  
  public color get()
  {
    color cl = color(r*255, g*255, b*255, a*255);
    return cl;
  }
  
  public Color combined(Color cl)
  {
    Color combined = new Color(r, g, b, a);
    combined.add(cl);
    return combined;
  }
  
  public Color add(Color cl)
  {
    r += cl.r;
    g += cl.g;
    b += cl.b;
    a += cl.a;
    
    constrain(r, 0, 1);
    constrain(g, 0, 1);
    constrain(b, 0, 1);
    constrain(a, 0, 1);
    return this;
  }
  
  public Color mult(float scalar)
  {
    r *= scalar;
    g *= scalar;
    b *= scalar;
    a *= scalar;
    return this;
  }
  
  public Color mult(Color cl)
  {
    r *= cl.r;
    g *= cl.g;
    b *= cl.b;
    a *= cl.a;
    return this;
  }
  
  public Color clone()
  {
    return new Color(r, g, b, a);
  }
  
  public String toString()
  {
    return "[ " + r*255 + "; " + g*255 + "; " + b*255 + "; " + a*255 + " ]";
  }
  
  public float r;
  public float g;
  public float b;
  public float a;
}
