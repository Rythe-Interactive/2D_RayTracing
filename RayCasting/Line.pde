public class Line
{
  public Line(PVector start, PVector dir)
  {
    this.start_ = start;
    this.dir_ = dir.normalize();
    
    normal_ = new PVector(-dir.y, dir.x);
    normal_.normalize();
  }
  
  public void set(PVector start, PVector dir)
  {
    this.start_ = start;
    this.dir_ = dir;
    
    normal_ = new PVector(-dir.y, dir.x);
    normal_.normalize();
  }
  
  public void render()
  {
    line(start_.x, start_.y, start_.x+dir_.x*500, start_.y+dir_.y*500);
  }
  
  public boolean collideWith(LineSegment other)
  {
    PVector startToStart = new PVector(other.end().x, other.end().y);
    startToStart.sub(start_);
    
    //project distance line on normal
    float distToLine = startToStart.dot(normal_);
    PVector line = vsub(other.start_, other.end_);
    float totalLine = -line.dot(normal_);
    
    float timeOfImpact = 1 - distToLine / totalLine;
    if(timeOfImpact < 0 || timeOfImpact > 1) return false;
    
    PVector vec = vscale(vsub(other.start_, other.end_), timeOfImpact);
    PVector poi = vsub(other.start_, vec);
    
    //we may assume that the 'end' of the line is at the poi
    PVector toPoi = vsub(poi, start_);
    
    //check if poi is behind the line (before the start)
    if(vnormalized(toPoi).dot(vnormalized(dir_)) < 0)
    {
      return false;
    }
    
    float distToPoi = toPoi.dot(vnormalized(vsub(poi, start_)));
    if(distToPoi < 0 || distToPoi > vlength(vsub(poi, start_)))
    {
      println("out of line");
      return false;
    }
    else
    {
      fill(0, 0, 255);
      ellipse(poi.x, poi.y, 10, 10);
      return true;
    }
  }
  
  //public boolean collideWith(PVector point, float radius)
  //{
  //  PVector startToCenter = new PVector(point.x, point.y);
  //  startToCenter.sub(start_);
    
  //  PVector line = vsub(end_, start_);
  //  float distOverLine = startToCenter.dot(vnormalized(line));
  //  if(distOverLine < -radius || distOverLine > vlength(line)+radius) return false;
    
  //  //project distance line on normal
  //  float dist = startToCenter.dot(normal_);
  //  if(abs(dist) < radius) return true;
  //  else return false;
  //}
  
  public PVector start()
  {
    return new PVector(start_.x, start_.y);
  }
  
  public PVector dir()
  {
    return new PVector(dir_.x, dir_.y);
  }
  
  public PVector normal()
  {
    return new PVector(normal_.x, normal_.y);
  }
  
  private PVector start_;
  private PVector dir_;
  private PVector normal_;
}
