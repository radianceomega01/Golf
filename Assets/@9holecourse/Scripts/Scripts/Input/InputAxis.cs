public struct InputAxis
{
    public float force;
    public float angle;

    private InputAxis(float force, float angle)
    {
        this.force = force;
        this.angle = angle;
    }

    public static InputAxis Default()
    {
        return new InputAxis(1, 0);
    }

    public static InputAxis CustomInput(float force, float angle)
    {
        return new InputAxis(force, angle);
    }
}
