package ModelProvider;

import org.eclipse.jetty.server.NCSARequestLog;
import org.eclipse.jetty.server.RequestLog;
import org.eclipse.jetty.server.Server;
import org.eclipse.jetty.server.handler.ContextHandlerCollection;
import org.eclipse.jetty.server.handler.HandlerCollection;
import org.eclipse.jetty.server.handler.RequestLogHandler;
import org.eclipse.jetty.servlet.ServletContextHandler;
import org.eclipse.jetty.servlet.ServletHolder;

import java.io.File;
import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.List;

//import static ModelProvider.Resources.foo;

/**
 * Created by bened on 11.06.2017.
 */

public class App {

    public static String path;
    public static String modelPath;

    private static final int major = 1;
    private static final int minor = 4;
    private static final int patch = 4;

    public static void main(String[] args) throws Exception {

        System.out.println("------- GaMR Model Provider Version " + major + "." + minor + "." + patch + " -------");

        try {
            path = Resources.ReadFile("config.conf");
            System.out.println("Base path: " + path);
            modelPath = path + File.separatorChar + "3DModels";
            System.out.println("3D Models at " + modelPath);
        }
        catch (IOException e)
        {
            System.out.println("Could not read the config file");
            return;
        }

        HandlerCollection handlers = new HandlerCollection();
        ContextHandlerCollection contexts = new ContextHandlerCollection();

        ServletContextHandler context = new ServletContextHandler(ServletContextHandler.SESSIONS);
        context.setContextPath("/");

        handlers.addHandler(context);

        Server jettyServer = new Server(8080);
        jettyServer.setHandler(handlers);

        // log all activity on the server
        NCSARequestLog requestLog = new NCSARequestLog();
        requestLog.setFilename("." + File.separatorChar + "logs" + File.separatorChar + "yyyy_mm_dd.request.log");
        requestLog.setFilenameDateFormat("yyyy_MM_dd");
        requestLog.setRetainDays(90);
        requestLog.setAppend(true);
        requestLog.setExtended(true);
        requestLog.setLogCookies(false);
        requestLog.setLogTimeZone("GMT");
        RequestLogHandler requestLogHandler = new RequestLogHandler();
        requestLogHandler.setRequestLog(requestLog);
        handlers.addHandler(requestLogHandler);


        ServletHolder jerseyServlet = context.addServlet(
                org.glassfish.jersey.servlet.ServletContainer.class, "/*");
        jerseyServlet.setInitOrder(0);

        // Tells the Jersey Servlet which REST service/class to load.
        jerseyServlet.setInitParameter(
                "jersey.config.server.provider.classnames",
                Resources.class.getCanonicalName());

        try {
            jettyServer.start();
            jettyServer.join();
        } finally {
            jettyServer.destroy();
        }
    }
}
