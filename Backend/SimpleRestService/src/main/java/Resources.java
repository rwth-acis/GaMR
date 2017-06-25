import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.File;
import java.io.IOException;
import java.util.Scanner;

/**
 * Created by bened on 11.06.2017.
 */

@Path("/resources")
public class Resources {

    @GET
    @Path("model/{no}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getModel(@PathParam("no") int no)
    {
        try {
            String json =  ReadFile("C:\\Temp\\3DModels\\" + no + ".json");
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("/texture/{name}")
    @Produces("image/jpg")
    public Response getTexture(@PathParam("name") String name)
    {
            File file = new File("C:\\Temp\\3DModels\\" + name);
            if (file.exists()) {
                return Response.ok(file, "image/jpg").header("Inline", "filename=\"" + file.getName() + "\"")
                        .build();
            }
            else {
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
    }


    private String ReadFile(String path) throws IOException
    {
        return new Scanner(new File(path)).useDelimiter("\\A").next();
    }
}
