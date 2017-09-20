package X3DConverter; /**
 * Created by bened on 12.06.2017.
 */

/**
 * Models a two or three-dimensional vector
 */
public class Vector {

    /** coordinates */
    protected float[] coords;


    /**
     * Creates a n-dimensional vector
     * @param coords the coordinates
     */
    public Vector(float... coords)
    {
        this.coords = coords;
    }

    public int dimension()
    {
        return coords.length;
    }

    public Float getX()
    {
        return GetValueOrNull(0);
    }

    public Float getY()
    {
        return GetValueOrNull(1);
    }

    public Float getZ()
    {
        return GetValueOrNull(2);
    }


    private Float GetValueOrNull(int index)
    {
        if (coords.length > index)
        {
            return coords[index];
        }
        else
        {
            return null;
        }
    }
}
