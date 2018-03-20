using System;
using UnityEngine;

public class FMOD_GetWaveDataDSP
{
    public static FMOD.DSP_DESCRIPTION CreateDSPDesc(IntPtr data) {
		var desc = new FMOD.DSP_DESCRIPTION()
		{
			name = "GetWaveData".ToCharArray(),
			version = 1,
			// TODO?: Instead of creating a dsp
			// for each channel, only create one
			// and set the number of buffers to
			// the number of channels.
			numinputbuffers = 1, 
			numoutputbuffers = 1,
			read = ReadAudioData,
			userdata = data,
		};

		return desc;
	}

	unsafe public static FMOD.RESULT ReadAudioData(ref FMOD.DSP_STATE dspState, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels) {
		FMOD.DSP dspInstance = new FMOD.DSP(dspState.instance);
		
		float *inb = (float *)inbuffer.ToPointer();
		float *outb = (float *)outbuffer.ToPointer();

		IntPtr userData;
		// NOTE: Without this check, Unity crashes!
		if (dspInstance.getUserData(out userData) == FMOD.RESULT.OK) {
			float *userBuffer = (float *) userData.ToPointer();

			// TODO: See if there is a better way to do this. 
			// Just copy back. 
			for (uint samp = 0; samp < length; samp++) 
			{ 
				/*
				  Feel free to unroll this.
				*/

				// TODO: There are at least 2 channels, what do we do?
				// Do we save the maximum, the average?
				userBuffer[samp] = inb[(samp * inchannels)];
			
				for (int chan = 0; chan < outchannels; chan++)
				{
					long outIdx = (samp * outchannels) + chan;
					long inIdx = (samp * inchannels) + chan;

					float value = inb[inIdx];
				
					outb[outIdx] = value;
				}
			}
		}

		return FMOD.RESULT.OK;
	}
}
