LineSegment leftTopLine;
LineSegment leftBottomLine;
LineSegment rightTopLine;
LineSegment rightBottomLine;

Ray ray;
Ray inverse;

void setup()
{
  smooth(8);
  leftTopLine = new LineSegment(new PVector(100, 50), new PVector(50, 250));
  leftTopLine.setColor(new Color(0.9, 0.5, 0));
  
  leftBottomLine = new LineSegment(new PVector(100, 450), new PVector(50, 250));
  leftBottomLine.setColor(new Color(0.2, 0.9, 0.1));
  
  rightTopLine = new LineSegment(new PVector(400, 50), new PVector(450, 250));
  rightTopLine.setColor(new Color(0, 0.5, 0.9));
  
  rightBottomLine = new LineSegment(new PVector(400, 450), new PVector(450, 250));
  rightBottomLine.setColor(new Color(0.9, 0.9, 0.9));
  
  ray = new Ray(new PVector(250, 250), new PVector(mouseX, mouseY), 1);
  ray.setColor(new Color(1, 1, 1));
  
  inverse = new Ray(new PVector(250, 250), new PVector(500-mouseX, 500-mouseY), 4);
  
  size(500, 500);
  frameRate(1000);
  
  textAlign(CENTER, CENTER);
}

void draw()
{
  background(0);
  
  PVector mouseDir = new PVector(mouseX-250, mouseY-250);
  mouseDir.normalize();
  ray.set(new PVector(250, 250).add(mouseDir), mouseDir );
  inverse.set(new PVector(250, 250).sub(vscale(mouseDir, -1)), vscale(mouseDir, -1));
  
  ray.collideWith(leftTopLine);
  ray.collideWith(leftBottomLine);
  ray.collideWith(rightTopLine);
  ray.collideWith(rightBottomLine);
  
  ray.renderNormal();
  
  //inverse.collideWith(leftTopLine[0]);
  //inverse.collideWith(leftTopLine[1]);
  //inverse.collideWith(leftBottomLine);
  //inverse.collideWith(rightTopLine);
  //inverse.collideWith(rightBottomLine);
  
  
  leftTopLine.render();
  leftTopLine.renderNormal();
  leftBottomLine.render();
  rightTopLine.render();
  rightBottomLine.render();
  ray.render();
  //inverse.render();
  
  fill(80);
  stroke(80);
  textSize(13);
  text(frameRate, 30, 10);
  fill(255);
  stroke(255);
  textSize(12);
  text(frameRate, 30, 10);
}
