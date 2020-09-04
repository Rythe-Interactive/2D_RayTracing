LineSegment leftTopLine;
LineSegment leftBottomLine;
LineSegment rightTopLine;
LineSegment rightBottomLine;
LineSegment topLine;
LineSegment bottomLine;

Ray[] ray;
int rayCount = 30;

int bounces = 4;

void setup()
{
  smooth(8);
  leftTopLine = new LineSegment(new PVector(101, 51), new PVector(49, 250));
  leftTopLine.setColor(new Color(0.9, 0.5, 0));
  
  leftBottomLine = new LineSegment(new PVector(100, 450), new PVector(50, 250));
  leftBottomLine.setColor(new Color(0.2, 0.9, 0.1));
  
  rightTopLine = new LineSegment(new PVector(400, 50), new PVector(450, 250));
  rightTopLine.setColor(new Color(0, 0.5, 0.9));
  
  rightBottomLine = new LineSegment(new PVector(400, 450), new PVector(450, 250));
  rightBottomLine.setColor(new Color(0.9, 0.9, 0.9));
  
  topLine = new LineSegment(new PVector(100, 50), new PVector(400, 50));
  topLine.setColor(new Color(1, 1, 0));
  
  bottomLine = new LineSegment(new PVector(100, 450), new PVector(400, 450));
  bottomLine.setColor(new Color(1, 0, 1));
  
  ray = new Ray[rayCount];
  for(int i = 0; i < rayCount; ++i)
  {
    PVector dir = PVector.fromAngle(PI*2/rayCount*i);
    ray[i] = new Ray(new PVector(0,0), dir, bounces);
  }
  
  size(500, 500);
  frameRate(1000);
  
  textAlign(CENTER, CENTER);
}

void draw()
{
  background(0);
  
  PVector mouseDir = new PVector(mouseX-250, mouseY-250);
  mouseDir.normalize();
  
  for(int i = 0; i < rayCount; ++i)
  {
    ray[i].set(vadd(new PVector(mouseX, mouseY), vscale(ray[i].dir(), 10)),ray[i].dir());
    
    ray[i].collideWith(topLine);
    ray[i].collideWith(rightTopLine);
    ray[i].collideWith(leftTopLine);
    ray[i].collideWith(leftBottomLine);
    ray[i].collideWith(rightBottomLine);
    ray[i].collideWith(bottomLine);
    
    ray[i].render();
  }
  
  topLine.render();
  leftTopLine.render();
  leftBottomLine.render();
  rightTopLine.render();
  rightBottomLine.render();
  bottomLine.render();
  
  fill(80);
  stroke(80);
  textSize(13);
  text(frameRate, 30, 10);
  text(bounces, width-30, 10);
  fill(255);
  stroke(255);
  textSize(12);
  text(frameRate, 30, 10);
  text(bounces, width-30, 10);
}

void keyPressed()
{
  if(key == '-')
  {
    --bounces;
  }
  if(key == '+')
  {
    ++bounces;
  }
  for(int i = 0; i < rayCount; ++i)
  {
    ray[i].setMaxBounces(bounces);
  }
}
