package fivebitencoding

import "strings"

// For Part-of-Speech:
func EncodePOS(input7charsMaxWithHyphen string) uint32 { // input string must be ascii
	l := len(input7charsMaxWithHyphen)
	if l < 1 || l > 7 {
		return 0
	}
	encoded := uint32(0x0)
	input := strings.TrimSpace(input7charsMaxWithHyphen)

	hyphen := uint32(strings.Index(input, "-"))
	if hyphen > 0 && hyphen <= 3 {
		hyphen <<= 30
	} else if len(input7charsMaxWithHyphen) > 6 { // 6 characters max if a compliant hyphen is not part of the string
		return 0
	} else {
		hyphen = uint32(0)
	}
	sb := strings.Builder{}
	for i := 0; i < len(input); i++ {
		b := byte(input[i])
		switch b {
		case '-':
			continue
		case '0', '1', '2', '3', '4':
			b -= byte('0')
			b += byte(27)
		}
		sb.WriteByte(b)
	}
	str := strings.ToLower(sb.String())
	position := uint32(0x02000000)
	for i := 0; i < 6-len(str); i++ {
		position >>= 5
	}
	for i := 0; i < len(str); i++ {
		letter := byte(str[i] & 0x1F)
		if letter == byte(0) {
			break
		}
		encoded |= uint32(letter) * position
		position >>= 5
	}
	return uint32(encoded | hyphen)
}

//  For Part-of-Speech:
func DecodePOS(encoding uint32) string {
	sb := strings.Builder{}
	hyphen := uint32(encoding & 0xC0000000)
	if hyphen > 0 {
		hyphen >>= 30
	}
	index := 0
	for mask := uint32(0x1F << 25); mask >= 0x1F; mask >>= 5 {
		digit := encoding & mask >> (5 * (5 - index))
		index++
		if digit == 0 {
			continue
		}
		b := byte(digit)
		if b <= 26 {
			b |= 0x60
		} else {
			b -= byte(27)
			b += byte('0')
		}
		sb.WriteByte(b)
	}
	str := sb.String()
	if hyphen > 0 && hyphen < uint32(len(str)) {
		str = str[0:hyphen] + "-" + str[hyphen:]
	}
	return str
}

func Encode(input3charsMax string, maxSegments int) []uint16 { // input string must be ascii
	ld := len(input3charsMax)
	if ld < 1 || ld > 7 {
		return []uint16{0}
	}
	last := ld / 3
	if len(input3charsMax)%3 == 0 {
		last--
	}
	le := last + 1
	i := 0
	if le > maxSegments {
		bad := make([]uint16, 0, 0)
		return bad
	}
	encoded := make([]uint16, le, le)
	for i = 0; i < last; i++ {
		encoded[i] = 0x8000 // overflow-bit
	}
	encoded[last] = 0x0000 // termination flag (no overflow)

	onsetLen := ld - (3 * last)
	start := 3 - onsetLen
	position := uint16(0x0400)
	if onsetLen < 3 {
		position >>= start * 5
	}
	i = -1
	str := strings.ToLower(input3charsMax)
	for s := 0; s < len(encoded); s++ {
		for z := start; z < 3; z++ {
			i++
			b := byte(str[i])
			switch b {
			case '-':
				b = byte(27)
			case '\'':
				b = byte(28)
			case ',':
				b = byte(29)
			case '.', '!':
				b = byte(30) // no room at the inn
			case '?':
				b = byte(31)
			default:
				b &= byte(0x1F)
			}

			if b == byte(0) {
				break
			}
			encoded[s] |= uint16(uint16(b) * position)
			position >>= 5
		}
		start = 0
		position = 0x400
	}
	return encoded
}

func Decode(encoded []uint16) string {
	sb := strings.Builder{}
	le := len(encoded)

	if le > 1 || encoded[0] != 0 {
		mask := uint16(0x1F)

		for s := 0; s < len(encoded); s++ {
			segment := encoded[s]
			for bit := uint16(0x01 << 10); bit > 0; bit >>= 5 {
				masked := uint16(segment) & (bit * mask)
				digit := byte(masked / bit)

				if digit == 0 {
					continue
				}
				if digit <= 26 {
					digit |= 0x60
					sb.WriteByte(digit)
				} else {
					switch digit {
					case 27:
						sb.WriteByte('-')
					case 28:
						sb.WriteByte('\'')
					case 29:
						sb.WriteByte(',')
					case 30:
						sb.WriteByte('.')
					case 31:
						sb.WriteByte('?')
					}
				}
			}
			if (segment & 0x8000) == 0 {
				break // this is okay; overflow not set ... we're done even though the array may be bigger
			}
		}
	}
	return sb.String()
}
