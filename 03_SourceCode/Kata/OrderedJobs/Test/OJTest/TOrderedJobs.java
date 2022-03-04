package OJTest;

import OrderedJobs.OrderedJobs;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

public class TOrderedJobs {
    @Test
    public void ItshouldReturnAGivenA()
    {
        //arrange
        var sut_ = new OrderedJobs();
        sut_.registerJob("A");
        //act
        sut_.sort();
        //assert
        assertEquals("A",sut_.getList());
    }

    @Test
    void ItshouldReturnABgivenAB() {
        //arrange
        var sut_ = new OrderedJobs();
        sut_.registerJob("A");
        sut_.registerJob("B");
        //act
        sut_.sort();
        //assert
        assertEquals("AB",sut_.getList());
    }

    @Test
    void ItShouldReturnABCgivenABC() {
        //arrange
        var sut_ = new OrderedJobs();
        sut_.registerJob("A");
        sut_.registerJob("B");
        sut_.registerJob("C");
        //act
        sut_.sort();
        //assert
        assertEquals("ABC",sut_.getList());
    }

    @Test
    void ItShouldReturnABgivenBdebendsOnA() {
        //arrange
        var sut_ = new OrderedJobs();
        sut_.registerJob("B","A");

        sut_.sort();

        assertEquals("AB", sut_.getList());
    }

    @Test
    void ItShouldReturnABCGivenCDependsOnBAndBDependsOnA() {
        //arrange
        var sut_ = new OrderedJobs();
        sut_.registerJob("B","A");
        sut_.registerJob("C","B");

        sut_.sort();

        assertEquals("ABC", sut_.getList());
    }

    @Test
    void ItShouldReturnABGivenABB() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("A");
        sut_.registerJob("B");
        sut_.registerJob("B");

        sut_.sort();

        assertEquals("AB", sut_.getList());
    }

    @Test
    void ItShouldReturnABC_GivenCdependsOnBandBdependsOnA() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("C","B");
        sut_.registerJob("B","A");

        sut_.sort();

        assertEquals("ABC", sut_.getList());
    }

    @Test
    void ItShoudReturnBA_GivenBAUsingSort() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("B");
        sut_.registerJob("A");

        sut_.sort();

        assertEquals("BA", sut_.getList());
    }

    @Test
    void ItShoudReturnBA_Given() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("B");
        sut_.registerJob("A");

        sut_.sort();

        assertEquals("BA", sut_.getList());
    }

    @Test
    void ItShouldReturnABCD_GivenDdependsOnCandCdependsOnBandBdependsOnA() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("D","C");
        sut_.registerJob("C","B");
        sut_.registerJob("B","A");

        sut_.sort();

        assertEquals("ABCD", sut_.getList());
    }

    @Test
    void ItShouldReturnEABCD_GivenEandDdependsOnCandCdependsOnBandBdependsOnA() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("E");
        sut_.registerJob("D","C");
        sut_.registerJob("C","B");
        sut_.registerJob("B","A");

        sut_.sort();

        assertEquals("EABCD", sut_.getList());
    }

    @Test
    void ItShouldReturnEAFBCD_GivenEandDdependsOnCandCdependsOnBandBdependsOnAandF() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("E");
        sut_.registerJob("D","C");
        sut_.registerJob("C","B");
        sut_.registerJob("B","A");
        sut_.registerJob("F");
        sut_.sort();

        assertEquals("EAFBCD", sut_.getList());
    }

    @Test
    void ItShouldReturnABCED_GivenDdependsOnCandCdependsOnBandBdependsOnAandDdependsOnEandEdependsOnA() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("D","C");
        sut_.registerJob("C","B");
        sut_.registerJob("B","A");
        sut_.registerJob("D","E");
        sut_.registerJob("E","A");

        sut_.sort();

        assertEquals("ABCED", sut_.getList());
    }

    @Test
    void test() {
        var sut_ = new OrderedJobs();
        sut_.registerJob("C","B");
        sut_.registerJob("D","E");
        sut_.registerJob("E","A");
        sut_.registerJob("D","C");
        sut_.registerJob("B","A");

        sut_.sort();

        assertEquals("ABCED", sut_.getList());
    }
}
