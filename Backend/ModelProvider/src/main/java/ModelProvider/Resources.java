package ModelProvider;

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
            String modelDirPath = ModelProvider.App.modelPath + File.separatorChar + name + File.separatorChar;
            String path = ModelProvider.App.modelPath + File.separatorChar + name + File.separatorChar + no + ".json";
            File dir = new File(modelDirPath);
            File[] x3ds = dir.listFiles(new FileFilter() {
                public boolean accept(File pathname) {
                    return  pathname.getName().endsWith(".x3d");
                }
            });

            File jsonCached = new File(path);
            System.out.println("Request for " + path);

            if (x3ds.length > 0)
            {
                if (!jsonCached.exists() || jsonCached.lastModified() < x3ds[0].lastModified())
                {
                    System.out.println("Creating/Updating model cache...");
                    // if the cached version does not exist or the x3d file is newer: first convert
                    X3DConverter.App.main(new String[]{"-i", x3ds[0].getPath(), "-o", modelDirPath});
                }
                String json = ReadFile(path);
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            }
            else // the x3d file does not exist
            {
                System.out.println("Warning: The X3D file for " + name + " was not found.");
                if (jsonCached.exists()) // if there is still a cached version => use it
                {
                    System.out.println("Using the cached version instead of X3D");
                    String json = ReadFile(path);
                    return Response.ok(json, MediaType.APPLICATION_JSON).build();
                }
                else
                {
                    return Response.status(Response.Status.BAD_REQUEST).build();
                }
            }
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
   @Path("/model/{name}/thumbnail")
    @Produces("image/png")
    public Response getThumbnail(@PathParam("name") String modelName)
    {
        File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "Thumbnail.png");
        if (file.exists()) {
            return Response.ok(file, "image/png").header("Inline", "filename=\"" + file.getName() + "\"")
                    .build();
        }
        else {
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
            File[] dirs = new File(App.modelPath).listFiles(new FileFilter() {
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
    @Produces("image/*")
    public Response getTexture(@PathParam("modelName") String modelName, @PathParam("name") String name)
    {
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + name);
            if (file.exists()) {
                return Response.ok(file, "image/*").header("Inline", "filename=\"" + file.getName() + "\"")
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
        System.out.println("Saving annotations for " + modelName);
        File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "annotations.json");
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
            String json =  ReadFile(App.modelPath + File.separatorChar + modelName + File.separatorChar + "annotations.json");
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("/quiz/overview/{modelName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getQuizOverview(@PathParam("modelName") String modelName)
    {
        try {
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "Quizzes"
                    + File.separatorChar);
            if (file.exists()) {
                File[] files = file.listFiles(new FileFilter() {
                    public boolean accept(File pathname) {
                        return pathname.getName().endsWith(".json");
                    }
                });
                CustomJSONArray jsonArray = new CustomJSONArray(files.length);
                for (int i = 0; i < files.length; i++) {
                    jsonArray.array[i] = files[i].getName().replaceAll(".json", "");
                }
                ObjectMapper mapper = new ObjectMapper();
                String json = mapper.writeValueAsString(jsonArray);
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            }
            else
            {
                // no quizzes created to this point => just return an empty array
                CustomJSONArray jsonArray = new CustomJSONArray(0);
                ObjectMapper mapper = new ObjectMapper();
                String json = mapper.writeValueAsString(jsonArray);
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            }
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("/quiz/load/{modelName}/{quizName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getQuiz(@PathParam("modelName") String modelName, @PathParam("quizName") String quizName)
    {
        try {
            String json = ReadFile(App.modelPath + File.separatorChar + modelName + File.separatorChar
                    + "Quizzes" + File.separatorChar + quizName + ".json");
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        }
        catch (IOException ioEx)
        {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @POST
    @Path("/quiz/save/{modelName}/{quizName}")
    @Consumes({ MediaType.APPLICATION_JSON })
    @Produces(MediaType.APPLICATION_JSON)
    public Response storeQuiz( @PathParam("modelName") String modelName, @PathParam("quizName") String quizName,
                               String json )
    {
        System.out.println("Saving quiz " + quizName + " for model " + modelName);
        File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "Quizzes" +
                File.separatorChar + quizName + ".json");
        try {
            file.getParentFile().mkdirs();
            FileWriter writer = new FileWriter(file);
            writer.write(json);
            writer.close();
            return  Response.status(Response.Status.CREATED).build();
        }
        catch (IOException e)
        {
            System.out.println(e.getMessage());
            return  Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
        }
    }

    @GET
    @Path("/badges/overview")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getBadgeImageOverview() {
        try {
            File[] images = new File(App.path + File.separatorChar + "BadgeImages" + File.separatorChar)
                    .listFiles(new FilenameFilter() {
                                   public boolean accept(File pathname, String name) {
                                       return  name.toLowerCase().endsWith(".jpg");
                                   }
                               });
                            CustomJSONArray jsonArray = new CustomJSONArray(images.length);
            for (int i = 0; i < images.length; i++) {
                jsonArray.array[i] = images[i].getName().replaceAll(".jpg", "");
            }
            ObjectMapper mapper = new ObjectMapper();
            String json = mapper.writeValueAsString(jsonArray);
            return Response.ok(json, MediaType.APPLICATION_JSON).build();
        } catch (IOException ioEx) {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }

    @GET
    @Path("/badges/{badgeName}")
    @Produces("image/jpg")
    public Response getBadgeTexture(@PathParam("badgeName") String badgeName)
    {
        File file = new File(App.path + File.separatorChar + "BadgeImages" + File.separatorChar + badgeName + ".jpg");
        if (file.exists()) {
            return Response.ok(file, "image/jpg").header("Inline", "filename=\"" + file.getName() + "\"")
                    .build();
        }
        else {
            return Response.status(Response.Status.BAD_REQUEST).build();
        }
    }


    public static String ReadFile(String path) throws IOException
    {
        return new Scanner(new File(path)).useDelimiter("\\A").next();
    }
}
