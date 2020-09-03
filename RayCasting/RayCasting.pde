LineSegment leftTopLine;
LineSegment leftBottomLine;
LineSegment rightTopLine;
LineSegment rightBottomLine;

Ray ray;

LineSegment rand;
LineSegment randTwin;

void setup()
{
  leftTopLine = new LineSegment(new PVector(50, 50), new PVector(50, 450));
  leftBottomLine = new LineSegment(new PVector(50, 450), new PVector(50, 50));
  rightTopLine = new LineSegment(new PVector(450, 50), new PVector(450, 450));
  rightBottomLine = new LineSegment(new PVector(450, 450), new PVector(450, 50));
  
  ray = new Ray(new PVector(250, 250), new PVector(mouseX, mouseY), 4);
  
  rand = new LineSegment(new PVector(250, 100), vscale(PVector.random2D(), 500));
  randTwin = new LineSegment(rand.end(), rand.start());
  
  size(500, 500);
}

void draw()
{
  background(255);
  
  ray.set(ray.start(), new PVector(mouseX-250, mouseY-250));
  
  stroke(0);
  leftTopLine.render();
  leftBottomLine.render();
  rightTopLine.render();
  ray.render();
  //rand.render();
  //randTwin.render();
  
  //line.collideWith(rand);
  //line.collideWith(randTwin);
  ray.collideWith(leftTopLine);
  ray.collideWith(leftBottomLine);
  ray.collideWith(rightTopLine);
  ray.collideWith(rightBottomLine);
  //ray.collideWith(rand);
  //ray.collideWith(randTwin);
  //stroke(255, 0 ,0);
  //line.renderNormal();
  //other.renderNormal();
  
  text(frameRate, 5, 10);
}

void keyPressed()
{
  if(key == ' ')
  {
    rand.set(rand.start(), vscale(PVector.random2D(), 500));
    randTwin.set(rand.end(), rand.start());
  }
  
  if(key == 'd')
  {
    rand.set(rand.start(), vadd(rand.end(), new PVector(5, 0)));
    randTwin.set(rand.end(), rand.start());
  }
  if(key == 'a')
  {
    rand.set(rand.start(), vadd(rand.end(), new PVector(-5, 0)));
    randTwin.set(rand.end(), rand.start());
  }
  if(key == 's')
  {
    rand.set(rand.start(), vadd(rand.end(), new PVector(0, 5)));
    randTwin.set(rand.end(), rand.start());
  }
  if(key == 'w')
  {
    rand.set(rand.start(), vadd(rand.end(), new PVector(0, -5)));
    randTwin.set(rand.end(), rand.start());
  }
}
