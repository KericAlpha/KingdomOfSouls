package OrderedJobs;

import java.util.LinkedList;

public class Job implements Comparable{
    private String name;
    private LinkedList<Job> dependsOnList;
    //private Job dependsOn;

    public Job(String name, Job dependsOn) {
        this.name = name;
        //this.dependsOn = dependsOn;
        this.dependsOnList = new LinkedList<Job>();
        addDepend(dependsOn);
    }

    public Job(String name) {
        this.name = name;
        this.dependsOnList = new LinkedList<Job>();
    }

    public String getName()
    {
        return this.name;
    }

    /*public Job getDependsOn() {
        return this.dependsOn;
    }

    public void setDependsOn(Job dependsOn) {
        this.dependsOn = dependsOn;
    }*/

    @Override
    public String toString() {
        return name;
    }

    public int checkDependency()
    {
        Integer counter = 0;
        for(Job job : dependsOnList)
        {
            counter += job.checkDependency() + 1;
        }
        /*if(dependsOn != null)
        {
            return dependsOn.checkDependency() + 1;
        }*/
        return counter;
    }

    public void addDepend(Job job)
    {
        dependsOnList.add(job);
    }

    @Override
    public int compareTo(Object o) {
        Integer test = this.checkDependency();
        Job job = (Job) o;
        Integer compare = test.compareTo(((Job) o).checkDependency());
        System.out.println("This: " + this.name + " Object: " + job.name + " Compare: " + compare.toString());
        return compare;
    }
}
