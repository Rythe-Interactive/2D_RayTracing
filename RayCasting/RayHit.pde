class RayHit
{
  public RayHit(Ray reflect, PVector hit, PVector normal, Color clr)
  {
    this.reflect = reflect;
    this.hit = hit;
    this.normal = normal;
    this.hitColor = clr;
  }
  
  public Ray reflect;
  public PVector hit;
  public PVector normal;
  public Color hitColor;
}
