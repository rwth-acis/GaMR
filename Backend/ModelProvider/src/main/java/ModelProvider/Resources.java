package ModelProvider;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.sun.xml.internal.ws.policy.privateutil.PolicyUtils;
import jdk.nashorn.internal.runtime.Debug;
import org.eclipse.jetty.server.Authentication;
import sun.misc.IOUtils;

import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.ws.rs.*;
import javax.ws.rs.core.Application;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.*;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.sql.Savepoint;
import java.util.Scanner;

/**
 * Created by bened on 11.06.2017.
 */

@Path("/resources")
public class Resources {


    @GET
    @Path("model/{name}/{no}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getModel(@PathParam("name") String name, @PathParam("no") int no, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetModel(" + name + "[" + no + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            try {
                String modelDirPath = ModelProvider.App.modelPath + File.separatorChar + name + File.separatorChar;
                String path = ModelProvider.App.modelPath + File.separatorChar + name + File.separatorChar + no + ".json";
                File dir = new File(modelDirPath);
                File[] x3ds = dir.listFiles(new FileFilter() {
                    public boolean accept(File pathname) {
                        return pathname.getName().endsWith(".x3d");
                    }
                });

                File jsonCached = new File(path);
                Logger.Log(methodName, "Received request for " + path);

                if (x3ds.length > 0) {
                    if (!jsonCached.exists() || jsonCached.lastModified() < x3ds[0].lastModified()) {
                        Logger.Log(methodName, "Creating/Updating model cache...");
                        // if the cached version does not exist or the x3d file is newer: first convert
                        X3DConverter.App.main(new String[]{"-i", x3ds[0].getPath(), "-o", modelDirPath});
                    }
                    String json = ReadFile(path);
                    Logger.Log(methodName, "Successfully processed request");
                    return Response.ok(json, MediaType.APPLICATION_JSON).build();
                } else // the x3d file does not exist
                {
                    Logger.Log(methodName, "Warning: The X3D file for " + name + " was not found.");
                    if (jsonCached.exists()) // if there is still a cached version => use it
                    {
                        Logger.Log(methodName, "Using the cached version instead of X3D");
                        String json = ReadFile(path);
                        Logger.Log(methodName, "Successfully processed request");
                        return Response.ok(json, MediaType.APPLICATION_JSON).build();
                    } else {
                        Logger.Log(methodName, "No X3D file found and no cached version");
                        return Response.status(Response.Status.BAD_REQUEST).build();
                    }
                }
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Error during model load\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
   @Path("/model/{name}/thumbnail")
    @Produces("image/png")
    public Response getThumbnail(@PathParam("name") String modelName, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetThumbnail(" + modelName + ")";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "Thumbnail.png");
            if (file.exists()) {
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(file, "image/png").header("Inline", "filename=\"" + file.getName() + "\"")
                        .build();
            } else {
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("model/overview")
    @Produces(MediaType.APPLICATION_JSON)
    /**
     * gets all directories in the 3DModels folder
     * each directory contains one model
     */
    public Response getOverview(@HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetModelOverview";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) // authorization ok
        {
            try {
                File[] dirs = new File(App.modelPath).listFiles(new FileFilter() {
                    public boolean accept(File pathname) {
                        return pathname.isDirectory();
                    }
                });
                CustomJSONArray jsonArray = new CustomJSONArray(dirs.length);
                for (int i = 0; i < dirs.length; i++) {
                    jsonArray.array[i] = dirs[i].getName();
                }
                ObjectMapper mapper = new ObjectMapper();
                String json = mapper.writeValueAsString(jsonArray);
                Logger.Log(methodName,"Sucessfully processed request by " + info.name);
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Error while handling the request by " + info.name + "\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else // authorization failed
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/texture/{modelName}/{name}")
    @Produces("image/*")
    public Response getTexture(@PathParam("modelName") String modelName, @PathParam("name") String name, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetTexture(" + modelName + "[" + name+"])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + name);
            if (file.exists()) {
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(file, "image/*").header("Inline", "filename=\"" + file.getName() + "\"")
                        .build();
            } else {
                Logger.Log(methodName, "The texture does not exist");
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @POST
    @Path("/annotation/save/{modelName}")
    @Consumes({ MediaType.APPLICATION_JSON })
    @Produces(MediaType.APPLICATION_JSON)
    public Response storeAnnotations( @PathParam("modelName") String modelName, String json, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "StoreAnnotations(" + modelName + ")";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            Logger.Log(methodName, "Saving annotations for " + modelName);
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "annotations.json");
            try {
                FileWriter writer = new FileWriter(file);
                writer.write(json);
                writer.close();
                Logger.Log(methodName, "Successfully processed request");
                return Response.status(Response.Status.CREATED).build();
            } catch (IOException e) {
                Logger.Log(methodName, "Could not write annotation file\n" + e.getMessage());
                return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/annotation/load/{modelName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getAnnotations(@PathParam("modelName") String modelName, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetAnnotations(" + modelName + ")";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            try {
                String json = ReadFile(App.modelPath + File.separatorChar + modelName + File.separatorChar + "annotations.json");
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Could not load annotations\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/annotation/audio/{modelName}/{annotationId}")
    @Produces("audio/wav")
    public Response getAnnotationAudio(@PathParam("modelName") String modelName, @PathParam("annotationId") String annotationId, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetAnnotationAudio(" + modelName + "["  + annotationId + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            File audio = new File(App.modelPath + File.separatorChar + modelName +
                    File.separatorChar + "Audio" + File.separatorChar + annotationId + ".wav");
            Logger.Log(methodName, "Received request for " + audio.getPath());
            try {
                if (audio.exists()) {
                    Logger.Log(methodName, "Successfully processed request");
                    return Response.ok(audio, "audio/wav").build();
                } else {
                    Logger.Log(methodName, "The audio file " + audio.getPath() + " does not exist");
                    return Response.status(Response.Status.BAD_REQUEST).build();
                }
            } catch (Exception e) {
                Logger.Log(methodName, "Could not load audio\n" + e.getMessage());
                return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @POST
    @Path("/annotation/audio/{modelName}/{annotationId}")
    @Consumes(MediaType.APPLICATION_OCTET_STREAM)
    @Produces(MediaType.APPLICATION_JSON)
    public Response storeAnnotationAudio(@PathParam("modelName") String modelName, @PathParam("annotationId") String annotationId, byte[] audio, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "StoreAnnotationAudio(" + modelName + "[" + annotationId + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            Logger.Log(methodName, "Received audio file");
            try {
                // only save audio annotations of existing models
                File modelDir = new File(App.modelPath + File.separatorChar + modelName);
                if (!modelDir.exists()) {
                    Logger.Log(methodName, "Tried to save an audio annotation for a model that does not exist; abort");
                    return Response.status(Response.Status.BAD_REQUEST).build();
                }

                //check annotationId to avoid string injection
                if (annotationId.contains("/") || annotationId.contains("\\") || annotationId.contains("."))
                {
                    Logger.Log(methodName, "Aborting audio saving because annotation id contains unexpected symbol(s)");
                    return  Response.status(Response.Status.BAD_REQUEST).build();
                }

                // save file

                File audioFile = new File(App.modelPath + File.separatorChar + modelName +
                        File.separatorChar + "Audio" + File.separatorChar + annotationId + ".wav");
                if (audioFile.getParentFile().mkdirs()) {
                    Logger.Log(methodName, "Creating folder(s) to save audio");
                }
                Files.write(audioFile.toPath(), audio);
                // check if saving was successful
                if (audioFile.exists()) {
                    Logger.Log(methodName, "Successfully processed request");
                    return Response.ok().build();
                } else {
                    Logger.Log(methodName, "Saving the audio file failed; audio file does not exist");
                    return Response.serverError().build();
                }
            } catch (IOException e) {
                Logger.Log(methodName, "Could not store audio file\n" + e.getMessage());
                return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @DELETE
    @Path("/annotation/audio/{modelName}/{annotationId}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response deleteAnnotationAudio(@PathParam("modelName") String modelName, @PathParam("annotationId") String annotationId, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "DeleteAnnotationAudio(" + modelName + "["  + annotationId + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            File audio = new File(App.modelPath + File.separatorChar + modelName +
                    File.separatorChar + "Audio" + File.separatorChar + annotationId + ".wav");
            try {
                if (audio.exists()) {
                    Logger.Log(methodName, "Deleting " + audio.getPath());
                    audio.delete();
                    if (!audio.exists()) {
                        Logger.Log(methodName, "Successfully processed request");
                        return Response.ok().build();
                    }
                    else
                    {
                        Logger.Log(methodName, "Tried deleting file but it still exists");
                        return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
                    }
                } else {
                    Logger.Log(methodName, "Cannot delete audio file " + audio.getPath() + ". It does not exist.");
                    return Response.status(Response.Status.BAD_REQUEST).build();
                }
            } catch (Exception e) {
                Logger.Log(methodName, "Could not delete audio\n" + e.getMessage());
                return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }


    @GET
    @Path("/quiz/overview/{modelName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getQuizOverview(@PathParam("modelName") String modelName, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetQuizOverview(" + modelName + ")";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
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
                    Logger.Log(methodName, "Successfully processed request");
                    return Response.ok(json, MediaType.APPLICATION_JSON).build();
                } else {
                    // no quizzes created to this point => just return an empty array
                    CustomJSONArray jsonArray = new CustomJSONArray(0);
                    ObjectMapper mapper = new ObjectMapper();
                    String json = mapper.writeValueAsString(jsonArray);
                    Logger.Log(methodName, "Successfully processed request (no quizzes exist)");
                    return Response.ok(json, MediaType.APPLICATION_JSON).build();
                }
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Could not get quiz overview\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/quiz/load/{modelName}/{quizName}")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getQuiz(@PathParam("modelName") String modelName, @PathParam("quizName") String quizName, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetQuiz(" + modelName + "[" + quizName + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            try {
                String json = ReadFile(App.modelPath + File.separatorChar + modelName + File.separatorChar
                        + "Quizzes" + File.separatorChar + quizName + ".json");
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Could not get quiz\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @POST
    @Path("/quiz/save/{modelName}/{quizName}")
    @Consumes({ MediaType.APPLICATION_JSON })
    @Produces(MediaType.APPLICATION_JSON)
    public Response storeQuiz( @PathParam("modelName") String modelName, @PathParam("quizName") String quizName,
                               String json, @HeaderParam("access_token") String accessToken )
    {
        String methodName = "StoreQuiz(" + modelName + "[" + quizName + "])";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            Logger.Log(methodName, "Saving quiz " + quizName + " for model " + modelName);
            File file = new File(App.modelPath + File.separatorChar + modelName + File.separatorChar + "Quizzes" +
                    File.separatorChar + quizName + ".json");
            try {
                file.getParentFile().mkdirs();
                FileWriter writer = new FileWriter(file);
                writer.write(json);
                writer.close();
                Logger.Log(methodName, "Successfully processed request; Created Quiz");
                return Response.status(Response.Status.CREATED).build();
            } catch (IOException e) {
                Logger.Log(methodName, "Could not store quiz\n" + e.getMessage());
                return Response.status(Response.Status.INTERNAL_SERVER_ERROR).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/badges/overview")
    @Produces(MediaType.APPLICATION_JSON)
    public Response getBadgeImageOverview(@HeaderParam("access_token") String accessToken) {
        String methodName = "GetBadgeImageOverview";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            try {
                File[] images = new File(App.path + File.separatorChar + "BadgeImages" + File.separatorChar)
                        .listFiles(new FilenameFilter() {
                            public boolean accept(File pathname, String name) {
                                return name.toLowerCase().endsWith(".jpg");
                            }
                        });
                CustomJSONArray jsonArray = new CustomJSONArray(images.length);
                for (int i = 0; i < images.length; i++) {
                    jsonArray.array[i] = images[i].getName().replaceAll(".jpg", "");
                }
                ObjectMapper mapper = new ObjectMapper();
                String json = mapper.writeValueAsString(jsonArray);
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(json, MediaType.APPLICATION_JSON).build();
            } catch (IOException ioEx) {
                Logger.Log(methodName, "Could not get badge image\n" + ioEx.getMessage());
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    @GET
    @Path("/badges/{badgeName}")
    @Produces("image/jpg")
    public Response getBadgeTexture(@PathParam("badgeName") String badgeName, @HeaderParam("access_token") String accessToken)
    {
        String methodName = "GetBadgeTexture(" + badgeName + ")";
        UserInformation info = ValidateAccessToken(accessToken);
        if (info != null) {
            File file = new File(App.path + File.separatorChar + "BadgeImages" + File.separatorChar + badgeName + ".jpg");
            if (file.exists()) {
                Logger.Log(methodName, "Successfully processed request");
                return Response.ok(file, "image/jpg").header("Inline", "filename=\"" + file.getName() + "\"")
                        .build();
            } else {
                Logger.Log(methodName, "Badge texture does not exist");
                return Response.status(Response.Status.BAD_REQUEST).build();
            }
        }
        else
        {
            Logger.Log(methodName, "Denied unauthorized access");
            return Response.status(Response.Status.UNAUTHORIZED).build();
        }
    }

    /**
     * Validates the given access token at the learning layers website
     * @param accessToken The access token to validate
     * @return the information of the user if the access token is valid; null else
     */
    private UserInformation ValidateAccessToken(String accessToken)
    {
        try {
            if (accessToken == null) // if no access token was provided => authorization failed
            {
                return  null;
            }
            // avoid string injection
            if (accessToken.contains("&") || accessToken.contains("?"))
            {
                return null;
            }
            URL validationURL = new URL("https://api.learning-layers.eu/o/oauth2/userinfo?access_token=" + accessToken);
            HttpURLConnection connection = (HttpURLConnection) validationURL.openConnection();

            connection.setRequestMethod("GET");

            int responseCode = connection.getResponseCode();
            if (responseCode == 200)
            {
                BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                String responseContent = "";
                String line;
                while ((line = reader.readLine()) != null)
                {
                    responseContent += line;
                }
                reader.close();
                ObjectMapper mapper = new ObjectMapper();
                UserInformation info = mapper.readValue(responseContent, UserInformation.class);
                connection.disconnect();
                return info;
            }
            else
            {
                connection.disconnect();
                return null;
            }

        }
        catch (IOException e)
        {
            return null;
        }
    }


    public static String ReadFile(String path) throws IOException
    {
        return new Scanner(new File(path)).useDelimiter("\\A").next();
    }
}
