import com.fasterxml.jackson.databind.ObjectMapper;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.util.Scanner;

/**
 * Created by bened on 11.06.2017.
 */

@Path("/resources")
public class Resources {

    @GET
    @Path("model/{name}/{no}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getModel(@PathParam("name") String name, @PathParam("no") int no)
    {
        try {
            String json =  ReadFile("C:\\Temp\\3DModels\\" + name + "\\" + no + ".json");
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("model/overview")
    @Produces(MediaType.APPLICATION_JSON)
    /**
     * gets all directories in the 3DModels folder
     * each directory contains one model
     */
    public Response getOverview()
    {
        try {
            File[] dirs = new File("C:\\Temp\\3DModels\\").listFiles(new FileFilter() {
                                                                         public boolean accept(File pathname) {
                                                                             return pathname.isDirectory();
                                                                         }
                                                                     });
            CustomJSONArray jsonArray = new CustomJSONArray(dirs.length);
            for (int i=0;i<dirs.length;i++)
            {
                jsonArray.array[i] = dirs[i].getName();
            }
            ObjectMapper mapper = new ObjectMapper();
            String json = mapper.writeValueAsString(jsonArray);
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("/texture/{modelName}/{name}")
    @Produces("image/jpg")
    public Response getTexture(@PathParam("modelName") String modelName, @PathParam("name") String name)
    {
            File file = new File("C:\\Temp\\3DModels\\" + modelName + "\\" + name);
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
