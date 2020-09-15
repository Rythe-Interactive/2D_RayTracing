Scene scene;

void settings()
{
  size(500, 500, P2D);
  smooth(8);
}

void setup()
{
  scene = new ImageScene();
  scene.init();
  frameRate(1000);

  textAlign(CENTER, CENTER);
}

void draw()
{
  scene.update();
}

void keyPressed()
{
  scene.handleInput();
}
