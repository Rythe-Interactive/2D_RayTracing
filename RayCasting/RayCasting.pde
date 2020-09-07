Scene scene;

void setup()
{
  size(500, 500);
  scene = new ImageScene();
  scene.init();
  smooth(8);
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
