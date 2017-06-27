import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpression;
import javax.xml.xpath.XPathFactory;
import java.io.File;
import java.util.LinkedList;
import java.util.List;

/**
 * Created by bened on 11.06.2017.
 */
public class App {

    public static void main(String[] args) {

        String inputPath = null;
        String outputPath = null;
        OptionsMode mode = OptionsMode.UNSPECIFIED;

        // process args
        for (int i=0;i<args.length;i++)
        {
            if (args[i].equals("-i"))
            {
                if (mode == OptionsMode.UNSPECIFIED)
                {
                    mode = OptionsMode.INPATH;
                }
                else
                {
                    System.out.println("Unexpected -i");
                    return;
                }
            }
            else if (args[i].equals("-o"))
            {
                if (mode == OptionsMode.UNSPECIFIED)
                {
                    mode = OptionsMode.OUTPATH;
                }
                else
                {
                    System.out.println("Unexpected -o");
                    return;
                }
            }
            else
            {
                if (mode == OptionsMode.INPATH)
                {
                    inputPath = args[i];
                }
                else if (mode == OptionsMode.OUTPATH)
                {
                    outputPath = args[i];
                }
                else
                {
                    System.out.println("Unexpected options");
                    return;
                }
                mode = OptionsMode.UNSPECIFIED;
            }
        }

        if (inputPath == null || outputPath == null)
        {
            System.out.println("You must specify input and output path");
        }


        try {
            File xmlFile = new File(inputPath);
            DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
            Document doc = dBuilder.parse(xmlFile);

            doc.getDocumentElement().normalize();

            XPathFactory xPathFactory =XPathFactory.newInstance();
            XPath xPath = xPathFactory.newXPath();
            XPathExpression expr = xPath.compile("/X3D/Scene/Shape/IndexedFaceSet");
            NodeList nl = (NodeList) expr.evaluate(doc, XPathConstants.NODESET);

            List<X3DPiece> models = new LinkedList<X3DPiece>();

            long time = System.currentTimeMillis();

            for (int i=0;i<nl.getLength();i++)
            {
                String vertexCoords;
                String textureCoords;
                String textureIndex;
                String vertexIndex;
                String textureName;

                Element currentElem = (Element) nl.item(i);

                // get attributes from children
                vertexCoords = extractChildNodeAttribute(currentElem, "Coordinate", "point");
                textureCoords = extractChildNodeAttribute(currentElem, "TextureCoordinate", "point");

                // get own attributes
                textureIndex = currentElem.getAttribute("texCoordIndex");
                vertexIndex = currentElem.getAttribute("coordIndex");

                // get texture from sibling Appearance/ImageTexture
                Element parent = (Element) currentElem.getParentNode();
                Element appearance = (Element)parent.getElementsByTagName("Appearance").item(0);
                textureName = extractChildNodeAttribute(appearance, "ImageTexture", "url");

                X3DPiece x3d = new X3DPiece(vertexCoords, textureCoords, vertexIndex, textureIndex, i, nl.getLength(), textureName);
                models.add(x3d);
            }

            System.out.println("Conversion took " + (System.currentTimeMillis() - time) + " ms");

            System.out.println(models.size() + " models converted");


            // convert object to json
            ObjectMapper mapper = new ObjectMapper();
            mapper.setSerializationInclusion(JsonInclude.Include.NON_NULL);

            for (int i=0;i<models.size();i++)
            {
                File outputFile = new File(outputPath + "\\" + i + ".json");
                // create all directories which don't exist
                outputFile.getParentFile().mkdirs();
                mapper.writeValue(outputFile, models.get(i));
            }
            System.out.println("Exported " + models.size() + " models to json");
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }


    }


    private static String extractChildNodeAttribute(Element parent, String childName, String attributeName)
    {
        Node childNode = parent.getElementsByTagName(childName).item(0);
        return childNode.getAttributes().getNamedItem(attributeName).getNodeValue();
    }
}
