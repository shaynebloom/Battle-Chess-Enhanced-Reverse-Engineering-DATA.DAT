# Battle-Chess-Enhanced-Reverse-Engineering-DATA.DAT

## Motivation

1) Learn a bit of C#

2) As a kid I used to play this game "Battle Chess Enhanced".  One interesting thing about the way the game was developed is the cd-rom would could play as a regular cd and it would play all the music tracks and battle sequence sound files.  What was interesting about this, was that there were sound files for battle sequences that were not in the game.  Because of these easter egg sound files, I would wonder if the developers had some added secret sprites or animation files as well.  I was never able to find anything playing the game, so I became interested in reverse engineering the data.dat file.  And that is how this project was born.

## DATA.DAT Structure

All of the animation files appear to be stored in the DATA.DAT file.  The .DAT extention didn't provide any clues on how to open the file so I ended up using trial and error, editting bytes with a hex editor and seeing what effects it had.

The following terminology will be used throughout this document:

Chunk - The .dat file appears to be a bunch of files merged together.  Some files do not have headers, which meant manually finding out what these files do, without any documentation to help.  Because of this, these will be referred to as chunks, they are chunks of data seperated by padding

Padding - Each chunk is followed by padding, which is a series of bytes spelling out Greg.  This is perhaps a reference to one of the games programmers, Greg Christensen.  The padding always ends with a newline (based on 16 byte lines) and the bytes are always lined up at specific points on the line (ie the last byte of the line will always be 'g', second to last will always be 'e').

## First and second chunks:

The first chunk is a list of 4 byte long little endian offsets.  These offsets tell you where every single chunk begins.  The second chunk is a list of 4 byte long little endian chunk lengths (in bytes).  These first two chunks are parallel in the sense that the first offset coorelates to the first chunk size, the second offset to the second chunk size, and so on.

Four of the following chunks have headers, and were the first thing I figured out.  They are .RIX files, which are old bitmap type image files.  The images are for the title screen splash image, and the board images (background).  RIX files are well documented online so they won't be covered here, but the color pallete part of them appears to be used for all the animation chunks.

This is where the fun began.  Every other image file is missing a header, so the only way to discover what they did was to change bytes and note the effect.  Luckily, all the animation files share the same format except for pixel data compression type, so once one was decoded, most of the others were pretty much solved.

## Animation files structure

***All offsets are relative to the chunk, not the DATA.DAT file***

The first four bytes (1-4) are the frame data offset in little endian
The second four bytes (5-8) are the animation data offset in little endian
These bytes are followed by the pixel data, bytes that say what pixels go where
After the pixel data is the animation data, which was pointed to by the offset previously
Finally the animation data, which was also pointed to earlier


### Animation data:
Lets begin with the animation data, since it's the simplest.  The animation data is a list of frame numbers, in the order they appear, followed by an unknown byte.  If the frame number and the unknown byte are both FF, that means there is a break in the animation.  Some chunks have multiple breaks, which seem to coorelate to parts where the animation switches to another.

### Frame data:
The frame data is a series of bytes that gives information on how to construct each frame.  Each frame has 16 bytes worth of information in this section.  The first 16 bytes of this section appear to be useless, so those are skipped.  In order to get the number of frames in an animation, subtract the frame data offset and 16 (remove first 16 bytes) from the animation data offset, then divide by 16.

As stated before each frame's information is 16 bytes, and are used for the following (all in little endian):

- The first two bytes are pixel width of the frame
- The next two bytes are pixel height of the frame
- The next two bytes appear to be the x coordinate for the placement of the frame, but sometimes this number is not correct, there seems to be more to it
- The next two bytes appear to be the y coordinate for the placement of the frame, but sometimes this number is not correct, there seems to be more to it
- The next four bytes are an offset in the pixel data section, which says where the pixel data for this specific frame begin
- The next four bytes are the byte length of that pixel data for this specific frame.
- The last byte represents the compression type of the pixel data.  There are three compression types.


