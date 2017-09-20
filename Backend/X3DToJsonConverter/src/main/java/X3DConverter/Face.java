package X3DConverter; /**
 * Created by bened on 12.06.2017.
 */

/**
 * Models a face which is made up of a number of vertices
 */
public class Face {

    /** the index of vertices which form a face */
    private Integer[] index;

    /**
     * Stores the indices of the vertices which form a face
     * @param index indices which form a face
     */
    public Face(Integer... index)
    {
        this.index = index;
    }

    public Integer[] getIndex() {
        return index;
    }

    public int getSize()
    {
        return index.length;
    }
}
