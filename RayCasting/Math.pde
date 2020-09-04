float vlength(PVector vec)
{
  float l = sqrt(pow(vec.x, 2) + pow(vec.y, 2));
  return l;
}

PVector vadd(PVector vec, PVector other)
{
  PVector copy = new PVector(vec.x, vec.y);
  return copy.add(other);
}

PVector vsub(PVector vec, PVector other)
{
  PVector copy = new PVector(vec.x, vec.y);
  return copy.sub(other);
}

PVector vnormalized(PVector vec)
{
  PVector copy = new PVector(vec.x, vec.y);
  return copy.normalize();
}

PVector vscale(PVector vec, float scale)
{
  PVector copy = new PVector(vec.x, vec.y);
  return copy.mult(scale);
}

float vcross(PVector vec0, PVector vec1)
{
  return vec0.x * vec1.y - vec0.y * vec1.x;
}

PVector vreflect(PVector vec, PVector normal)
{
  PVector n = new PVector(normal.x, normal.y);
  PVector reflect = vsub(vec, vscale(n, 2* n.dot(vec)));
  return reflect;
}

float degToRad(float deg)
{
  return deg / 180 * PI;
}
