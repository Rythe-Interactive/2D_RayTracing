public class Ray
{
  public Ray(PVector start, PVector dir, int maxBounces)
  {
    this.start_ = start;
    this.dir_ = dir.normalize();

    normal_ = new PVector(-dir.y, dir.x);
    normal_.normalize();

    bounces_ = new ArrayList<Ray>();
    lineSegments_ = new ArrayList<LineSegment>();
    maxBounces_ = maxBounces;
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
    for (int i = 0; i < bounces_.size(); ++i)
    {
      bounces_.get(i).render();
    }
    bounces_.clear();
    lineSegments_.clear();

    if (hasEnd_) line(start_.x, start_.y, end_.x, end_.y);
    else line(start_.x, start_.y, start_.x+dir_.x*500, start_.y+dir_.y*500);
    hasEnd_ = false;
  }

  public boolean collideWith(LineSegment other)
  {
    lineSegments_.add(other);
    for (int i = 0; i < bounces_.size(); ++i)
    {
      bounces_.get(i).collideWith(other);
    }

    PVector startToStart = new PVector(other.end().x, other.end().y);
    startToStart.sub(start_);

    //project distance line on normal
    float distToLine = startToStart.dot(normal_);
    PVector line = vsub(other.start_, other.end_);
    float totalLine = -line.dot(normal_);

    float timeOfImpact = 1 - distToLine / totalLine;
    if (timeOfImpact < 0 || timeOfImpact > 1) return false;

    PVector otherStartToPoi = vscale(vsub(other.end_, other.start_), timeOfImpact);
    PVector poi = vadd(other.start_, otherStartToPoi);

    if (vlength(otherStartToPoi) > vlength(vsub(other.end_, other.start_)))
    {
      //println("poi is over the end of line");
      return false;
    }

    //this can also be seen as the full line from here
    PVector thisToPoi = vsub(poi, start_);
    if (thisToPoi.dot(vnormalized(dir_)) < 0)
    {
      //println("poi was before the start of the ray");
      return false;
    }

    float poiDistanceOverOther = thisToPoi.dot(vnormalized(line));
    if (poiDistanceOverOther < 0)
    {
      //println("poi was under the start of the line: " + poiDistanceOverOther);
      return false;
    } else
    {
      if (hasEnd_)
      {
        if (vlength(vsub(end_, start_)) < vlength(vsub(poi, start_)))
        {
          return true;
        }
      }
      fill(0, 0, 255);
      ellipse(poi.x, poi.y, 10, 10);
      end_ = poi;
      hasEnd_ = true;

      if (maxBounces_ != 0)
      {
        //make a new ray as a bounce to this one
        PVector reflectDir = vnormalized(vreflect(dir_, other.normal()));
        Ray reflect = new Ray(vadd(poi, vscale(reflectDir, 5)), reflectDir, maxBounces_-1);
        bounces_.add(reflect);
        for (int i = 0; i < lineSegments_.size(); ++i)
        {
          reflect.collideWith(lineSegments_.get(i));
        }
      }

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
  private PVector end_;
  private boolean hasEnd_ = false;
  private PVector normal_;
  private int maxBounces_;

  private ArrayList<Ray> bounces_;
  private ArrayList<LineSegment> lineSegments_;
}
