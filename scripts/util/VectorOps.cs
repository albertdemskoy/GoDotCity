using Godot;

public static class VectorOps {
    public static Vector3 Lerp(Vector3 first, Vector3 second, float a) {
        return new Vector3(
            Mathf.Lerp(first.X,second.X,a), 
            Mathf.Lerp(first.Y,second.Y,a), 
            Mathf.Lerp(first.Y,second.Y,a));
    }
    public static Vector3 Lerp(Vector3 first, Vector3 second, double a) {
        return new Vector3(
            Mathf.Lerp(first.X,second.X,(float)a), 
            Mathf.Lerp(first.Y,second.Y,(float)a), 
            Mathf.Lerp(first.Y,second.Y,(float)a));
    }
}