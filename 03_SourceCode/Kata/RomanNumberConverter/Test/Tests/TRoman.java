package Tests;

import Source.RomanNumberConverter;
import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

public class TRoman {
    @Test
    void ItShouldReturnIGiven1() {
        assertEquals("I", RomanNumberConverter.toRoman(1));
    }

    @Test
    void ItShouldReturnIIGiven2() {
        assertEquals("II",RomanNumberConverter.toRoman(2));
    }

    @Test
    void ItShouldReturnIIIGiven3() {
        assertEquals("III",RomanNumberConverter.toRoman(3));
    }

    @Test
    void ItShouldReturnVGiven5() {
        assertEquals("V",RomanNumberConverter.toRoman(5));
    }

    @Test
    void ItShouldReturnVIsGivenBetween6and8() {
        assertEquals("VI",RomanNumberConverter.toRoman(6));
        assertEquals("VII",RomanNumberConverter.toRoman(7));
    }

    @Test
    void ItShouldReturnXGiven10() {
        assertEquals("X",RomanNumberConverter.toRoman(10));
    }

    @Test
    void ItShouldReturnXIGiven11() {
        assertEquals("XI",RomanNumberConverter.toRoman(11));
    }

    @Test
    void ItShouldReturnXXGiven20()  {
        assertEquals("XX",RomanNumberConverter.toRoman(20));
    }

    @Test
    void ItShouldReturnIVGiven4() {
        assertEquals("IV",RomanNumberConverter.toRoman(4));
    }

    @Test
    void ItShouldReturnIXGiven9() {
        assertEquals("IX",RomanNumberConverter.toRoman(9));
    }

    @Test
    void ItShouldReturn1GivenI() {
        assertEquals(1,RomanNumberConverter.toArabic("I"));
    }

    @Test
    void ItShouldReturn2GivenII() {
        assertEquals(2,RomanNumberConverter.toArabic("II"));
    }

    @Test
    void ItShouldReturn5GivenV() {
        assertEquals(5,RomanNumberConverter.toArabic("V"));
    }

    @Test
    void ItShouldReturn6GivenVI() {
        assertEquals(6,RomanNumberConverter.toArabic("VI"));
    }

    @Test
    void ItShouldReturn4GivenIV() {
        assertEquals(4,RomanNumberConverter.toArabic("IV"));
    }

    @Test
    void ItShouldReturn9GivenIX() {
        assertEquals(9,RomanNumberConverter.toArabic("IX"));
    }
}


