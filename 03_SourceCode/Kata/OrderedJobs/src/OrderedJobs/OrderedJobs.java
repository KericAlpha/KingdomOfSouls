package OrderedJobs;

import java.util.*;

public class OrderedJobs {

    private LinkedList<Job> jobs = new LinkedList<Job>();

    public void registerJob(String jobName)
    {
        boolean isAlreadyRegistered = false;
        for(Job job : jobs)
        {
            if(job.getName().equals(jobName))
            {
                isAlreadyRegistered = true;
            }
        }

        if(!isAlreadyRegistered)
        {
            jobs.add(new Job(jobName));
        }
    }

    public void registerJob(String dependent,String independent)
    {

        boolean isAlreadyRegistered = false;

        Job job = new Job(independent);
        for(Job jobOld : jobs)
        {
            if(jobOld.getName().equals(job.getName()))
            {
                job = jobOld;
                isAlreadyRegistered = true;
            }
        }

        if(!isAlreadyRegistered)
        {
            jobs.add(job);
        }

        Job job2 = new Job(dependent);
        isAlreadyRegistered = false;

        for(Job jobOld : jobs)
        {
            if(Objects.equals(jobOld.getName(), job2.getName()))
            {
                jobOld.addDepend(job);
                isAlreadyRegistered = true;
            }
        }

        if(!isAlreadyRegistered)
        {
            job2 = new Job(dependent, job);
            jobs.add(job2);
        }
    }

    public void sort()
    {
        Collections.sort(jobs);
    }

    public String getList()
    {
        String jobsInOrderOfDependency = "";
        for(Job job : jobs)
        {
            jobsInOrderOfDependency += job.toString();
        }
        return jobsInOrderOfDependency;
    }
}
