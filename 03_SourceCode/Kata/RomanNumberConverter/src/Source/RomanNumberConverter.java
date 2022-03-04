package Source;

public class RomanNumberConverter {

    public static void main(String[] args) {
        System.out.println(Numeral.ONE.mRoman + Numeral.ONE.mRoman);
    }

    public static String toRoman(int aNumber)
    {
        String romanNumber = "";
        for (var numeral : Numeral.values()){
            while(aNumber >= numeral.mArabic) {
                romanNumber += numeral.mRoman;
                aNumber -= numeral.mArabic;
            }
        }
        return romanNumber;
    }

    public static int toArabic(String roman)
    {
        int arabic = 0;

        for (int i = 0; i < roman.length() ; i++)
        {
            for(var numeral : Numeral.values())
            {
                if(i < roman.length()-1)
                {
                    if((Character.toString(roman.charAt(i)) + Character.toString(roman.charAt(i + 1))).equals(numeral.mRoman))
                    {
                        arabic += numeral.mArabic;
                        i ++;
                    }
                }
                if(Character.toString(roman.charAt(i)).equals(numeral.mRoman))
                {
                    arabic += numeral.mArabic;
                }
            }
        }

        return arabic;
    }
    enum Numeral{
        TEN(10, "X"),
        NINE(9,"IX"),
        FIVE(5,"V"),
        FOUR(4,"IV"),
        ONE(1,"I");

        private final int mArabic;
        private final String mRoman;

        Numeral(int arabic, String roman) {
            mArabic = arabic;
            mRoman = roman;
        }
    }
}
