import stc from 'string-to-color';
import chroma from 'chroma-js';

export const getForegroundColor = (input: string, backgroundColor: string, contrastThreshold: number = 4.5): string => {
    const bgColor = chroma(backgroundColor);

    let inputString = input;
    let fgColor = "";
    let contrast = 0;

    do {
        fgColor = stc(inputString);
        contrast = chroma.contrast(bgColor, fgColor);
        inputString += "-";
    } while (contrast < contrastThreshold)

    return fgColor;
};
