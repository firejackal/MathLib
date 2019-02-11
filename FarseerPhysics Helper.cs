using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;

public static class FarseerPhysicsHelper
{
    public static void SetAsBox(PolygonShape poly, float hx, float hy)
    {
        Vertices points = new Vertices();
        points.Add(new Vector2(-hx, -hy));
        points.Add(new Vector2(hx, -hy));
        points.Add(new Vector2(hx, hy));
        points.Add(new Vector2(-hx, hy));
        poly.Vertices = points;

        /*poly.Normals.Clear();
        poly.Normals.Add(new Vector2(0.0F, -1.0F));
        poly.Normals.Add(new Vector2(1.0F, 0.0F));
        poly.Normals.Add(new Vector2(0.0F, 1.0F));
        poly.Normals.Add(new Vector2(-1.0F, 0.0F));*/
    } //SetAsBox

    public static void SetAsBox(PolygonShape poly, float hx, float hy, Vector2 center, float angle)
    {
        Vertices polys = new Vertices();
        polys.Add(new Vector2(-hx, -hy));
        polys.Add(new Vector2(hx, -hy));
        polys.Add(new Vector2(hx, hy));
        polys.Add(new Vector2(-hx, hy));
        //poly.Normals.Add(new Vector2(0.0F, -1.0F));
        //poly.Normals.Add(new Vector2(1.0F, 0.0F));
        //poly.Normals.Add(new Vector2(0.0F, 1.0F));
        //poly.Normals.Add(new Vector2(-1.0F, 0.0F));

        //_centroid = center;

        Transform xf = new Transform(); //(center, angle)
        xf.p = center;
        xf.q.Set(angle);

        // Transform vertices and normals.
        for(int i = 0; i < polys.Count; i++) {
            polys[i] = Multiply(xf, polys[i]);
            //poly.Normals(i) = Multiply(xf.R, poly.Normals(i))
        } //i

        poly.Vertices = polys;
    } //SetAsBox

    public static PolygonShape CreateBox(float density, float hx, float hy)
    {
        PolygonShape result = new PolygonShape(density);
        SetAsBox(result, hx, hy);
        return result;
    } //CreateBox

    public static PolygonShape CreateBox(float density, float hx, float hy, Vector2 center, float angle)
    {
        PolygonShape result = new PolygonShape(density);
        SetAsBox(result, hx, hy, center, angle);
        return result;
    } //CreateBox

    public static PolygonShape CreateRoundedBox(float density, float halfWidth, float halfHeight, float edgeWidth, float edgeHeight)
    {
        PolygonShape poly = new PolygonShape(density);

        Vertices points = new Vertices();
        points.Add(new Vector2(-halfWidth + edgeWidth, -halfHeight)); // bottom
        points.Add(new Vector2(halfWidth - edgeWidth, -halfHeight)); // bottom-right edge start
        points.Add(new Vector2(halfWidth, -halfHeight + edgeHeight)); // bottom-right edge end
        points.Add(new Vector2(halfWidth, halfHeight)); // top-right
        points.Add(new Vector2(-halfWidth, halfHeight)); // top-left
        points.Add(new Vector2(-halfWidth, -halfHeight + edgeHeight)); // bottom-left edge
        poly.Vertices = points;

        return poly;
    } //CreateRoundedBox function

    public static Vector2 Multiply(Transform T, Vector2 v)
    {
        float x = T.p.X + T.q.GetXAxis().X * v.X + T.q.GetYAxis().X * v.Y;
        float y = T.p.Y + T.q.GetXAxis().Y * v.X + T.q.GetYAxis().Y * v.Y;

        return new Vector2(x, y);
    } //Multiply
} // FarseerPhysicsHelper class
