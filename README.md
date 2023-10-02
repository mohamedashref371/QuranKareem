# Quran Kareem v1.1
With 40 audio Quran and 10 tafasir

Requirements to run the program: [.NET Framework 4.8](https://go.microsoft.com/fwlink/?linkid=2088631)

To download this program with code: ([pre-release](https://github.com/mohamedashref371/QuranKareem/archive/refs/heads/master.zip))

I did not care about designing the form or adding forms yet, and instead I used the Guna UI 2.0 library.

Wanting to revive an idea from 2002 ([Quran-CD-Roaya-5](https://archive.org/download/QuranCDRoaya5/Quran-CD-Roaya-5.iso)), I built a mostly standardized SQLite database model.

And to ensure myself correct data, I did the following:

- I used [UthmanicHafs_v2-1](https://fonts.qurancomplex.gov.sa/wp02/حفص) from [King Fahd Glorious Qur'an Printing Complex](https://qurancomplex.gov.sa/) to get the written Qur’an and convert it to [sqlite database](https://github.com/mohamedashref371/QuranKareem/blob/master/data/texts/حفص%20عن%20عاصم.db).

- I extracted At-tafasir with some modifications from [KSU Ayat v1.4](https://quran.ksu.edu.sa/ayat/) to my database model.

- I used the json files provided by this link to obtain the timing of the end of the verses:
https://api.qurancdn.com/api/qdc/audio/reciters/{sheikh_number}/audio_files?chapter={surah_number}&segments=true <br><br>Some of the audio surahs contain iste3athah, and the json files start from the first verse. It may happen that the program accidentally reads the iste3athah with basmalah when reciting basmalah. This is not a problem with the database model structure and this will be updated later.


You can now add any sheikh you want smoothly and easily through the program.<br>
Databases are publicly readable, so you can add Al-Mashaykh in any other way you prefer.

The current program is just an idea that can be improved and developed.
