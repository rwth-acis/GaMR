import com.fasterxml.jackson.databind.ObjectMapper;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.*;
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
            System.out.println(json);
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            System.out.println("error");
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

    @POST
    @Path("/annotation/save/{modelName}")
    @Consumes({ MediaType.APPLICATION_JSON })
    @Produces(MediaType.APPLICATION_JSON)
    public Response storeAnnotations( @PathParam("modelName") String modelName, String json )
    {
        System.out.println(modelName + ": " + json);
        File file = new File("C:\\Temp\\3DModels\\" + modelName + "\\" + "annotations.json");
        try {
            FileWriter writer = new FileWriter(file);
            writer.write(json);
            writer.close();
            return  Response.status(Response.Status.CREATED).build();
        }
        catch (IOException e)
        {
            return  Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GET
    @Path("/annotation/load/{modelName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getAnnotations(@PathParam("modelName") String modelName)
    {
        try {
            String json =  ReadFile("C:\\Temp\\3DModels\\" + modelName + "\\annotations.json");
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }


    private String ReadFile(String path) throws IOException
    {
        return new Scanner(new File(path)).useDelimiter("\\A").next();
    }
}
