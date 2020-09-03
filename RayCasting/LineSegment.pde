public class LineSegment
{
  public LineSegment(PVector start, PVector end)
  {
    this.start_ = start;
    this.end_ = end;
    
    PVector line = new PVector(start.x, start.y);
    line.sub(end);
    normal_ = new PVector(-line.y, line.x);
    normal_.normalize();
  }
  
  public void set(PVector start, PVector end)
  {
    this.start_ = start;
    this.end_ = end;
    
    PVector line = new PVector(start.x, start.y);
    line.sub(end);
    normal_ = new PVector(-line.y, line.x);
    normal_.normalize();
  }
  
  public void render()
  {
    line(start_.x, start_.y, end_.x, end_.y);
  }
  
  public void renderNormal()
  {
    PVector line = new PVector(end_.x, end_.y);
    line.sub(start_);
    line = line.mult(0.5f);
    PVector point = new PVector(start_.x, start_.y);
    point.add(line);
    line(point.x, point.y, point.x + normal_.x * 20, point.y + normal_.y * 20);
  }
  
  public boolean collideWith(LineSegment other)
  {
    PVector startToStart = new PVector(other.end_.x, other.end_.y);
    startToStart.sub(start_);
    
    //project distance line on normal
    float distToLine = startToStart.dot(normal_);
    PVector line = vsub(other.start_, other.end_);
    float totalLine = -line.dot(normal_);
    
    float timeOfImpact = 1 - distToLine / totalLine;
    if(timeOfImpact < 0 || timeOfImpact > 1) return false;
    
    PVector vec = vscale(vsub(other.start_, other.end_), timeOfImpact);
    PVector poi = vsub(other.start_, vec);
    PVector toPoi = vsub(poi, start_);
    float distToPoi = toPoi.dot(vnormalized(vsub(end_, start_)));
    println(distToPoi);
    if(distToPoi < 0 || distToPoi > vlength(vsub(end_, start_))) return false;
    else
    {
    fill(0, 0, 255);
      ellipse(poi.x, poi.y, 10, 10);
      return true;
    }
  }
  
  public boolean collideWith(PVector point, float radius)
  {
    PVector startToCenter = new PVector(point.x, point.y);
    startToCenter.sub(start_);
    
    PVector line = vsub(end_, start_);
    float distOverLine = startToCenter.dot(vnormalized(line));
    if(distOverLine < -radius || distOverLine > vlength(line)+radius) return false;
    
    //project distance line on normal
    float dist = startToCenter.dot(normal_);
    if(abs(dist) < radius) return true;
    else return false;
  }
  
  public PVector start()
  {
    return new PVector(start_.x, start_.y);
  }
  
  public PVector end()
  {
    return new PVector(end_.x, end_.y);
  }
  
  public PVector normal()
  {
    return new PVector(normal_.x, normal_.y);
  }
  
  private PVector start_;
  private PVector end_;
  private PVector normal_;
}
