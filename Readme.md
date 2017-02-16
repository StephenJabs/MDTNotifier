MDT Notifier is a helper application for MDT 2013 that reads the Application Event Log, looking for "MDT_Monitor" events 2, 3, 41015 and 41016, and sends notifications to the designated email address.

2 = Deployment warning  
3 = Deployment error  
41015 = Deployment started  
41016 = Deployment completed successfully


It works in conjunction with a scheduled task that monitors the application log for these events.  See MDT_Monitor.xml for an example task configuration that can be imported, or manually follow these steps:

Step 1.
Create a scheduled task in Task Scheduler.

Step 2.
Set four triggers for "On an event" that look in the Application log for source MDT_Monitor and the specified Event ID's.

Step 3.
Create an action for "Start a Program" that runs MDTNotifier.exe with arguments web.config and a Start in location of the application.

Step 4.
Save the task, providing credentials to an account that has permissions to run the task whether the user is logged on or not.
