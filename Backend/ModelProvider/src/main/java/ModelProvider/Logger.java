package ModelProvider;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Logger {

    private static DateFormat timestamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

    public  static  void Log(String message)
    {
        Date currentDate = new Date();
        System.out.println("[" + timestamp.format(currentDate) + "] " + message);
    }

    public static void Log(String sender, String message)
    {
        Log(sender + ": " + message);
    }
}
