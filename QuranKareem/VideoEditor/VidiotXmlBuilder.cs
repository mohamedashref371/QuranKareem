using System;
using static QuranKareem.VideoXMLProperties;
using static QuranKareem.Constants;

namespace QuranKareem
{
    // download from https://sourceforge.net/projects/vidiot
    internal static class VidiotXmlBuilder
    {
        private static int object_id;
        private static int obj_id_x, obj_id_y, obj_id_z;
        private static int frameNum = 30;
        private static int frameDiv = 1;
        private static readonly string dataTime = "2023-10-07T06:00:00";
        private static int vOffset, aOffset, length;

        public static string Build()
        {
            StrBuilder.Clear();

            frameNum = (int)FrameRate;
            frameDiv = 1;
            if (FrameRate != frameNum)
            {
                frameNum = (int)Math.Round(FrameRate * 100);
                frameDiv = 100;
            }

            vOffset = (int)(VideoOffsetInSecond * FrameRate);
            aOffset = (int)(AudioOffsetInSecond * FrameRate);
            length = (int)(LengthInSecond * FrameRate);

            XmlStart();

            AddAudioFile();
            object_id = 5;
            AddImageFiles();
            AddVideoFile();

            SequencesStart();

            VideoInitialize();
            ImagesInitialize();
            AudioInitialize();

            Output();

            return StrBuilder.ToString();
        }

        private static void XmlStart()
        {
            StrBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?><!DOCTYPE boost_serialization><boost_serialization signature=\"serialization::archive\" version=\"19\"><project class_id=\"0\" tracking_level=\"1\" version=\"3\" object_id=\"_0\"><mProperties class_id=\"1\" tracking_level=\"0\" version=\"1\"><px class_id=\"2\" tracking_level=\"1\" version=\"4\" object_id=\"_1\"><mFrameRate class_id=\"3\" tracking_level=\"0\" version=\"1\"><rational64 class_id=\"4\" tracking_level=\"0\" version=\"0\"><numerator>")
              .Append(frameNum)
              .Append("</numerator><denominator>")
              .Append(frameDiv)
              .Append("</denominator></rational64></mFrameRate><mVideoWidth>")
              .Append(VideoWidth)
              .Append("</mVideoWidth><mVideoHeight>")
              .Append(VideoHeight)
              .Append("</mVideoHeight><mAudioChannels>")
              .Append(AudioChannels)
              .Append("</mAudioChannels><mAudioSampleRate>")
              .Append(AudioSampleRate)
              .Append("</mAudioSampleRate></px></mProperties><mMetaDataCache class_id=\"5\" tracking_level=\"0\" version=\"1\"><px class_id=\"6\" tracking_level=\"1\" version=\"4\" object_id=\"_2\"><mMetaData class_id=\"7\" tracking_level=\"0\" version=\"0\"><count>")
              .Append(ImagesPaths.Count + 2)
              .Append("</count><item_version>0</item_version>");
        }

        private static void AddAudioFile()
        {
            StrBuilder.Append("<item class_id=\"8\" tracking_level=\"0\" version=\"0\"><first class_id=\"9\" tracking_level=\"0\" version=\"2\"><filename class_id=\"10\" tracking_level=\"0\" version=\"0\"><string>")
              .Append(AudioPath)
              .Append("</string></filename></first><second class_id=\"11\" tracking_level=\"0\" version=\"1\"><px class_id=\"12\" tracking_level=\"1\" version=\"4\" object_id=\"_3\"><LastModified class_id=\"13\" tracking_level=\"0\" version=\"0\"><datetime><string>")
              .Append(dataTime)
              .Append("</string></datetime></LastModified><Length class_id=\"14\" tracking_level=\"0\" version=\"1\"><initialized>0</initialized></Length><FrameRate class_id=\"15\" tracking_level=\"0\" version=\"1\"><initialized>0</initialized></FrameRate><Peaks class_id=\"16\" tracking_level=\"0\" version=\"1\"><px class_id=\"17\" tracking_level=\"1\" version=\"2\" object_id=\"_4\"><nPeaks>0</nPeaks><peaks></peaks></px></Peaks></px></second></item>");
        }

        private static void AddImageFiles()
        {
            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                StrBuilder.Append("<item><first><filename><string>")
                  .Append(ImagesPaths[i])
                  .Append("</string></filename></first><second><px class_id_reference=\"12\" object_id=\"_")
                  .Append(object_id++)
                  .Append("\"><LastModified><datetime><string>")
                  .Append(dataTime)
                  .Append("</string></datetime></LastModified><Length><initialized>0</initialized></Length><FrameRate><initialized>1</initialized><value><rational64><numerator>")
                  .Append(frameNum)
                  .Append("</numerator><denominator>")
                  .Append(frameDiv)
                  .Append("</denominator></rational64></value></FrameRate><Peaks><px class_id=\"-1\"></px></Peaks></px></second></item>");
            }
        }

        private static void AddVideoFile()
        {
            StrBuilder.Append("<item><first><filename><string>")
              .Append(VideoPath)
              .Append("</string></filename></first><second><px class_id_reference=\"12\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><LastModified><datetime><string>")
              .Append(dataTime)
              .Append("</string></datetime></LastModified><Length><initialized>0</initialized></Length><FrameRate><initialized>1</initialized><value><rational64><numerator>")
              .Append(frameNum)
              .Append("</numerator><denominator>")
              .Append(frameDiv)
              .Append("</denominator></rational64></value></FrameRate><Peaks><px class_id_reference=\"17\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><nPeaks>0</nPeaks><peaks></peaks></px></Peaks></px></second></item></mMetaData></px></mMetaDataCache>");
        }

        private static void SequencesStart()
        {
            obj_id_x = object_id++;
            StrBuilder.Append("<mSequences class_id=\"18\" tracking_level=\"0\" version=\"0\"><count>1</count><item_version>1</item_version><item class_id=\"19\" tracking_level=\"0\" version=\"1\"><px class_id=\"20\" tracking_level=\"1\" version=\"4\" object_id=\"_")
              .Append(obj_id_x)
              .Append("\"><IAudio class_id=\"21\" tracking_level=\"0\" version=\"1\"></IAudio><mName><string>")
              .Append(ProjectName)
              .Append("</string></mName><mVideoTracks class_id=\"22\" tracking_level=\"0\" version=\"0\"><count>2</count><item_version>1</item_version>");
        }

        private static void VideoInitialize()
        {
            StrBuilder.Append("<item class_id=\"23\" tracking_level=\"0\" version=\"1\"><px class_id=\"25\" class_name=\"model::VideoTrack\" tracking_level=\"1\" version=\"1\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><Track class_id=\"24\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mIndex>0</mIndex><mClips class_id=\"26\" tracking_level=\"0\" version=\"0\"><count>1</count><item_version>1</item_version><item class_id=\"27\" tracking_level=\"0\" version=\"1\"><px class_id=\"28\" class_name=\"model::VideoClip\" tracking_level=\"1\" version=\"4\" object_id=\"_");

            obj_id_y = object_id++;
            StrBuilder.Append(obj_id_y)
              .Append("\"><ClipInterval class_id=\"29\" tracking_level=\"0\" version=\"6\"><Clip class_id=\"30\" tracking_level=\"0\" version=\"2\"><mLink class_id=\"31\" tracking_level=\"0\" version=\"0\"><ptr><px class_id=\"32\" class_name=\"model::AudioClip\" tracking_level=\"1\" version=\"6\" object_id=\"_");

            obj_id_z = object_id++;
            StrBuilder.Append(obj_id_z)
              .Append("\"><ClipInterval><Clip><mLink><ptr><px class_id_reference=\"28\" object_id_reference=\"_")
              .Append(obj_id_y)
              .Append("\"></px></ptr></mLink></Clip><mSource class_id=\"33\" tracking_level=\"0\" version=\"1\"><px class_id=\"34\" class_name=\"model::AudioSourceAvcodec\" tracking_level=\"1\" version=\"1\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mPath><filename><string>")
              .Append(VideoPath)
              .Append("</string></filename></mPath><mPreferredStreamIndex class_id=\"35\" tracking_level=\"0\" version=\"1\"><initialized>0</initialized></mPreferredStreamIndex></px></mSource><mSpeed><numerator>1</numerator><denominator>1</denominator></mSpeed><mOffset>")
              .Append(vOffset)
              .Append("</mOffset><mLength>")
              .Append(length)
              .Append("</mLength><mKeyFrames class_id=\"36\" tracking_level=\"0\" version=\"0\"><count>0</count><item_version>0</item_version></mKeyFrames><mDefaultKeyFrame class_id=\"37\" tracking_level=\"0\" version=\"1\"><px class_id=\"39\" class_name=\"model::AudioKeyFrame\" tracking_level=\"1\" version=\"3\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mVolume>0</mVolume><mBalance>0</mBalance></px></mDefaultKeyFrame><mSyncOffset>0</mSyncOffset></ClipInterval><IAudio></IAudio></px></ptr></mLink></Clip><mSource><px class_id=\"40\" class_name=\"model::VideoSourceMovie\" tracking_level=\"1\" version=\"1\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mPath><filename><string>")
              .Append(VideoPath)
              .Append("</string></filename></mPath><mFrameRate><initialized>0</initialized></mFrameRate><mPreferredStreamIndex><initialized>0</initialized></mPreferredStreamIndex></px></mSource><mSpeed><numerator>1</numerator><denominator>1</denominator></mSpeed><mOffset>")
              .Append(vOffset)
              .Append("</mOffset><mLength>")
              .Append(length)
              .Append("</mLength><mKeyFrames><count>0</count><item_version>0</item_version></mKeyFrames><mDefaultKeyFrame><px class_id=\"41\" class_name=\"model::VideoKeyFrame\" tracking_level=\"1\" version=\"6\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mInputSize class_id=\"42\" tracking_level=\"0\" version=\"0\"><x>")
              .Append(VideoWidth)
              .Append("</x><y>")
              .Append(VideoHeight)
              .Append("</y></mInputSize><mOpacity>255</mOpacity><mScaling>1</mScaling><mScalingFactor><numerator>1</numerator><denominator>1</denominator></mScalingFactor><mRotation><numerator>0</numerator><denominator>1</denominator></mRotation><mFlipHorizontal>0</mFlipHorizontal><mAlignment>0</mAlignment><mPosition class_id=\"43\" tracking_level=\"0\" version=\"0\"><x>0</x><y>0</y></mPosition><mCropTop>0</mCropTop><mCropBottom>0</mCropBottom><mCropLeft>0</mCropLeft><mCropRight>0</mCropRight></px></mDefaultKeyFrame><mSyncOffset>0</mSyncOffset></ClipInterval><IVideo class_id=\"44\" tracking_level=\"0\" version=\"1\"></IVideo></px></item></mClips><mHeight>51</mHeight></Track><IVideo></IVideo></px></item>");
        }

        private static void ImagesInitialize()
        {
            StrBuilder.Append("<item><px class_id_reference=\"25\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><Track object_id=\"_")
              .Append(object_id++)
              .Append("\"><mIndex>1</mIndex><mClips><count>")
              .Append(ImagesPaths.Count)
              .Append("</count><item_version>1</item_version>");

            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                StrBuilder.Append("<item><px class_id_reference=\"28\" object_id=\"_")
                  .Append(object_id++)
                  .Append("\"><ClipInterval><Clip><mLink><ptr><px class_id=\"-1\"></px></ptr></mLink></Clip><mSource><px ");
                if (i == 0)
                    StrBuilder.Append("class_id=\"45\" class_name=\"model::VideoSourceImage\" tracking_level=\"1\" version=\"2\"");
                else
                    StrBuilder.Append("class_id_reference=\"45\"");
                StrBuilder.Append(" object_id=\"_")
                .Append(object_id++)
                .Append("\"><mPath><filename><string>")
                .Append(ImagesPaths[i])
                .Append("</string></filename></mPath><mOffset>0</mOffset></px></mSource><mSpeed><numerator>1</numerator><denominator>1</denominator></mSpeed><mOffset>13823850</mOffset><mLength>")
                .Append(Math.Round(AudioTimestamps[i] * FrameRate)) // length 
                .Append("</mLength><mKeyFrames><count>0</count><item_version>0</item_version></mKeyFrames><mDefaultKeyFrame><px class_id_reference=\"41\" object_id=\"_")
                .Append(object_id++)
                .Append("\"><mInputSize><x>")
                .Append(VideoWidth)
                .Append("</x><y>")
                .Append(VideoHeight)
                .Append("</y></mInputSize><mOpacity>255</mOpacity><mScaling>")
                .Append("1")
                .Append("</mScaling><mScalingFactor><numerator>")
                .Append("1")
                .Append("</numerator><denominator>")
                .Append("1")
                .Append("</denominator></mScalingFactor><mRotation><numerator>0</numerator><denominator>1</denominator></mRotation><mFlipHorizontal>0</mFlipHorizontal><mAlignment>3</mAlignment><mPosition><x>")
                .Append("0")
                .Append("</x><y>")
                .Append("0")
                .Append("</y></mPosition><mCropTop>0</mCropTop><mCropBottom>0</mCropBottom><mCropLeft>0</mCropLeft><mCropRight>0</mCropRight></px></mDefaultKeyFrame><mSyncOffset>0</mSyncOffset></ClipInterval><IVideo></IVideo></px></item>");
            }

            StrBuilder.Append("</mClips><mHeight>53</mHeight></Track><IVideo></IVideo></px></item></mVideoTracks>");
        }

        private static void AudioInitialize()
        {
            StrBuilder.Append("<mAudioTracks><count>2</count><item_version>1</item_version><item><px class_id=\"46\" class_name=\"model::AudioTrack\" tracking_level=\"1\" version=\"1\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><Track object_id=\"_")
              .Append(object_id++)
              .Append("\"><mIndex>0</mIndex><mClips><count>1</count><item_version>1</item_version><item><px class_id_reference=\"32\" object_id_reference=\"_")
              .Append(obj_id_z)
              .Append("\"></px></item></mClips><mHeight>29</mHeight></Track><IAudio></IAudio></px></item><item><px class_id_reference=\"46\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><Track object_id=\"_")
              .Append(object_id++)
              .Append("\"><mIndex>1</mIndex><mClips><count>1</count><item_version>1</item_version><item><px class_id_reference=\"32\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><ClipInterval><Clip><mLink><ptr><px class_id=\"-1\"></px></ptr></mLink></Clip><mSource><px class_id_reference=\"34\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mPath><filename><string>")
              .Append(AudioPath)
              .Append("</string></filename></mPath><mPreferredStreamIndex><initialized>0</initialized></mPreferredStreamIndex></px></mSource><mSpeed><numerator>1</numerator><denominator>1</denominator></mSpeed><mOffset>")
              .Append(aOffset)
              .Append("</mOffset><mLength>")
              .Append(length)
              .Append("</mLength><mKeyFrames><count>0</count><item_version>0</item_version></mKeyFrames><mDefaultKeyFrame><px class_id_reference=\"39\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mVolume>100</mVolume><mBalance>0</mBalance></px></mDefaultKeyFrame><mSyncOffset>0</mSyncOffset></ClipInterval><IAudio></IAudio></px></item></mClips><mHeight>50</mHeight></Track><IAudio></IAudio></px></item></mAudioTracks>");
        }

        private static void Output()
        {
            StrBuilder.Append("<mRender class_id=\"47\" tracking_level=\"0\" version=\"1\"><px class_id=\"48\" tracking_level=\"1\" version=\"5\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><filename><string>")
              .Append(OutputPath)
              .Append("</string></filename><mOutputFormat class_id=\"49\" tracking_level=\"0\" version=\"1\"><px class_id=\"50\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mName><string>mp4</string></mName><mLongName><string>MPEG-4</string></mLongName><mExtensions class_id=\"51\" tracking_level=\"0\" version=\"0\"><count>1</count><item_version>0</item_version><item><string>mp4</string></item></mExtensions><mDefaultAudioCodecType>3</mDefaultAudioCodecType><mDefaultVideoCodecType>4</mDefaultVideoCodecType><mAudioCodec class_id=\"52\" tracking_level=\"0\" version=\"1\"><px class_id=\"53\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mType>3</mType><mParameters class_id=\"54\" tracking_level=\"0\" version=\"0\"><count>1</count><item_version>1</item_version><item class_id=\"55\" tracking_level=\"0\" version=\"1\"><px class_id=\"56\" class_name=\"model::render::AudioCodecParameterBitrate\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mValue>")
              .Append(AudioBitRate)
              .Append("</mValue></px></item></mParameters></px></mAudioCodec><mVideoCodec class_id=\"57\" tracking_level=\"0\" version=\"1\"><px class_id=\"58\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mType>4</mType><mParameters><count>2</count><item_version>1</item_version><item><px class_id=\"59\" class_name=\"model::render::VideoCodecParameterCrfH265\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mValue>28</mValue></px></item><item><px class_id=\"60\" class_name=\"model::render::VideoCodecParameterPreset\" tracking_level=\"1\" version=\"2\" object_id=\"_")
              .Append(object_id++)
              .Append("\"><mValue>3</mValue></px></item></mParameters></px></mVideoCodec></px></mOutputFormat><mMetaData class_id=\"61\" tracking_level=\"0\" version=\"0\"><count>0</count><item_version>0</item_version></mMetaData></px></mRender></px></item></mSequences></project><view class_id=\"62\" tracking_level=\"0\" version=\"1\"><window class_id=\"63\" tracking_level=\"0\" version=\"2\"><timelinesview class_id=\"64\" tracking_level=\"0\" version=\"1\"><notebookCount>1</notebookCount><selectedPage>0</selectedPage><sequence><px class_id_reference=\"20\" object_id_reference=\"_")
              .Append(obj_id_x)
              .Append("\"></px></sequence><timeline class_id=\"65\" tracking_level=\"0\" version=\"2\"><zoom class_id=\"66\" tracking_level=\"0\" version=\"2\"><mZoom><numerator>1</numerator><denominator>1</denominator></mZoom></zoom><intervals class_id=\"67\" tracking_level=\"0\" version=\"1\"><mIntervals class_id=\"68\" tracking_level=\"0\" version=\"0\"><listofpairs class_id=\"69\" tracking_level=\"0\" version=\"0\"><count>0</count><item_version>0</item_version></listofpairs></mIntervals></intervals><scrolling class_id=\"70\" tracking_level=\"0\" version=\"0\"><mCenterPts>0</mCenterPts></scrolling><cursor class_id=\"71\" tracking_level=\"0\" version=\"0\"><mCursorPosition>163</mCursorPosition></cursor><player class_id=\"72\" tracking_level=\"0\" version=\"0\"><speed>100</speed></player></timeline></timelinesview></window></view></boost_serialization>");
        }
    }
}
