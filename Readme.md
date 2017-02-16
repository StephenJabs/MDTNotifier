MDT Notifier is a helper application for MDT 2013 that reads the Application Event Log, looking for "MDT_Monitor" events 2, 3, 41015 and 41016, and sends notifications to the designated email address.

2 = Deployment warning  
3 = Deployment error  
41015 = Deployment started  
41016 = Deployment completed successfully

It works in conjunction with a scheduled task that monitors the application log for these events.
