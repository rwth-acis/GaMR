/**
 * Created by bened on 12.06.2017.
 */

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

/**
 * This class models a X3D object
 */
public class X3DPiece {


    /**
     * The number of this piece in the whole model
     */
    private int pieceIndex;

    /**
     * overall number of pieces in the whole model
     * (this X3DPiece is only one piece in the model)
     */
    private int pieceCount;

    /**
     * positions of the vertex coordinates
     */
    private Vector[] vertexCoords;
    /**
     * positions of the vertices on the texture
     */
    private Vector[] textureCoords;
    /**
     * index which describes which indices form a face
     */
    private int[] vertexIndex;
    /**
     * index which describes which indices of the textureCoords form a face in the texture
     */
    private int[] textureIndex;
    /**
     * name of the texture
     */
    private String textureName;

    /**
     * Creates a X3DPiece from the required strings
     * This constructor converts the strings into arrays of @see Class#Vector and float.
     * Those are then stored in the object.
     * All numbers in the strings need to be separated by a space
     *
     * @param vertexCoords  String which describes the three-dimensional coordinates of the vertices
     * @param textureCoords String which describes the two-dimensional coordinates of the vertices on the texture (in the range of 0 to 1)
     * @param vertexIndex   String which defines which vertices of vertexCoords form a face (faces are usually separated by a -1)
     * @param textureIndex  String which defines which vertices of textureCoords form a face (faces are usually separated by a -1)
     * @param pieceIndex    Index of the piece
     */
    public X3DPiece(String vertexCoords, String textureCoords, String vertexIndex, String textureIndex, int pieceIndex,
                    int pieceCount, String textureName)
            throws Exception {
        super();
        this.vertexCoords = convertStringToCoords(vertexCoords, 3);
        if (textureCoords == null ||textureIndex == null ||textureName == null) {
            // if one of those is null, the model will be untextured
            // => no need to convert any texture data which are present
            this.textureCoords = null;
            this.textureIndex = null;
            this.textureName = null;
        }
        else
        {
            this.textureCoords = convertStringToCoords(textureCoords, 2);
            this.textureIndex = convertStringToIndex(textureIndex);
            this.textureName = textureName;
        }
        this.vertexIndex = convertStringToIndex(vertexIndex);
        this.pieceIndex = pieceIndex;
        this.pieceCount = pieceCount;
    }

    /**
     * Converts a string of coordinates separated by a space into the corresponding array of vectors
     *
     * @param text      The string to convert (should contain float-numbers separated by spaces)
     * @param dimension The dimension of the coordinates (should be 2 for textureCoords or 3 for vertexCoords)
     * @return Array of Vectors which describe the coordinate positions
     */
    private Vector[] convertStringToCoords(String text, int dimension) throws Exception {

        Vector[] res;
        // split the string by spaces
        String[] parts = text.split("\\s+");

        if (parts.length % dimension == 0) {
            res = new Vector[parts.length / dimension];
        } else {
            throw new Exception();
        }

        // iterate over each vector (<dimension> many consecutive values)
        for (int i = 0; i < parts.length; i += dimension) {
            float[] coords = new float[dimension];
            // iterate through the values for the current vector
            for (int j = 0; j < dimension; j++) {
                coords[j] = Float.parseFloat(parts[i + j]);
            }
            Vector vec = new Vector(coords);
            res[i / dimension] = vec;
        }

        return res;
    }

    /**
     * @param text The string to convert (should contain integers separated by spaces)
     * @return Array of faces
     */
    private int[] convertStringToIndex(String text) {
        // split the string by spaces
        String[] parts = text.split("\\s+");

        int[] faceList = new int[parts.length];
        int counter = 0;

        for (int i = 0; i < parts.length; i++) {
            int converted = Integer.parseInt(parts[i]);

            // copy every value except for the -1
            if (converted != -1) {
                faceList[counter] = converted;
                counter++;
            }
        }

        return Arrays.copyOfRange(faceList, 0, counter);
    }


//    /**
//     *
//     * @param text The string to convert (should contain integers separated by spaces)
//     * @return Array of faces
//     */
//    private Face[] convertStringToIndex(String text)
//    {
//        List<Face> faceList = new LinkedList<Face>();
//        // split the string by spaces
//        String[] parts = text.split("\\s+");
//
//        List<Integer> face = new LinkedList<Integer>();
//        // iterate over each number
//        for(int i=0;i<parts.length;i++)
//        {
//            int converted = Integer.parseInt(parts[i]);
//
//            if (converted != -1)
//            {
//                face.add(converted);
//            }
//
//            // if -1 is reached or at the end of the list => organize as face
//            if (converted == -1 || i == parts.length -1)
//            {
//                Face f = new Face(face.toArray(new Integer[face.size()]));
//                faceList.add(f);
//                // also reset the buffer which stores the indices
//                face = new LinkedList<Integer>();
//            }
//        }
//
//        return faceList.toArray(new Face[faceList.size()]);
//    }

    public Vector[] getVertexCoords() {
        return vertexCoords;
    }

    public Vector[] getTextureCoords() {
        return textureCoords;
    }

    public int[] getVertexIndex() {
        return vertexIndex;
    }

    public int[] getTextureIndex() {
        return textureIndex;
    }

    public int getPieceIndex() {
        return pieceIndex;
    }

    public int getPieceCount() {
        return pieceCount;
    }

    public String getTextureName() {
        return textureName;
    }
}
