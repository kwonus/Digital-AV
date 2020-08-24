package main
//	Sample source code for Digital AV -- SDK 2017 Edition -- Example based upon revision #HA29
//	Copyright (c) 1996-2017 Kevin Wonus
import (
	"net/http"
	"io"
	"os"
	"strings"
	"strconv"
	"io/ioutil"
	"fmt"
	"crypto/md5"
	"hash"
	"bufio"
)
const
(
	PUNCclause	        = 0xE0
	PUNCexclamatory	    = 0x80
	PUNCinterrogative  	= 0xC0
	PUNCdeclarative   	= 0xE0
	PUNCdash  		    = 0xA0
	PUNCsemicolon       = 0x20
	PUNCcomma 		    = 0x40
	PUNCcolon      	    = 0x60
	PUNCpossessive    	= 0x10
	PUNCparagraph       = 0x08
	MODEparenthetical   = 0x04
	MODEitalics   	    = 0x02
	MODEjesus      	    = 0x01
)
const
(
	EndBit				= 0x1 // (0b0001____)
	VerseTransition		= 0x3 // (0b0010____)
	BeginingOfVerse		= 0x2 // (0b0010____)
	EndOfVerse			= 0x3 // (0b0011____)
	ChapterTransition 	= 0x7 // (0b0110____)
	BeginingOfChapter 	= 0x6 // (0b0110____)
	EndOfChapter		= 0x7 // (0b0111____)
)
const
(
	Version = "#HB25"
)
type book struct {
	chapterIdx uint16
	bookNum byte
	chapterCnt byte
	name string
	abbreviation string
	abbreviationMin byte
	alternate string
	alternateMin byte
}
type bibleText struct {
	strongs    []uint16
	verseIndex uint16
	wordKey    uint16
	puncuation byte
	transCase  byte
	pos        uint16
}
func fileExists(file string) bool {
	_, err := os.Stat(file)
	if err == nil {
		return true
	} else if os.IsNotExist(err) {
		return false
	}
	check(err)	// otherwise, panic
	return false;
}
func isFileType(file string, ext string) bool {
	info, err := os.Stat(file)
	if err != nil {
		return false
	} else if info.IsDir() {
		return strings.ToLower(ext) == "{dir}"
	}
	dot := strings.LastIndex(file, ".")
	if (dot >= 0) {
		test := file[dot:]
		return strings.ToLower(test) == strings.ToLower(ext)
	}
	return false
}
func replaceFileType(file string, newext string) string {
	dot := strings.LastIndex(file, ".")
	if (dot >= 1) {
		replacement := file[0:dot] + newext;
		return replacement
	}
	return ""
}

func hexDigit(c byte) byte {
	if c >= '0' && c <= '9' {
		return c - '0'
	}
	if (c >= 'A') && (c <= 'F') {
		return 0xA + c - 'A'
	}
	return 0xF
}
func getStylesheet(session string) string {
	assignments := strings.Split(session, "=")
	if len(assignments) >= 2 {
		//	Special-case where avspec has not been created, but supplied on the URL line as hexencoded []uint16 array
		//	Write it out here to unify processing.  This is useful for testing
		assignment := assignments[1]
		stylesheet := strings.ToLower(assignments[0])

		avspec := cssDir + "/" + stylesheet + ".avspec"
		if !fileExists(avspec) {
			fsession, err := os.Create(avspec)
			check(err)

			if (assignment[0] == '$') {
				strings.ToUpper(assignment)
				pair := make([]byte, 2)
				for i := 1; i+3 < len(assignment); i += 4 {
					pair[0] = hexDigit(assignment[i+0]) * 0x10
					pair[0] += hexDigit(assignment[i+1])
					pair[1] = hexDigit(assignment[i+2]) * 0x10
					pair[1] += hexDigit(assignment[i+3])
					val16 := convUInt16(pair)
					writeUInt16(fsession, val16)
				}
			} else {
				strings.ToLower(assignment)
				tokens := strings.Split(assignment, "&")
				writeUInt16(fsession, uint16(len(tokens)))
				for _, token := range tokens[0:] {
					writeUInt16(fsession, reverseLex(token))
				}
				writeUInt16(fsession, uint16(0))
			}
			fsession.Close()
		}
		return stylesheet
	}
	return session
}
func fourOfour(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	w.WriteHeader(http.StatusNotFound)
	fmt.Fprintf(w, "404! Not found")
}
func createMD5(path string, missingIsFatal bool) hash.Hash {
	file, err := os.Open(path)
	if (missingIsFatal) {
		check(err)
	} else if (err != nil) {
		return nil
	}
	chksum := md5.New()
	bytes := make([]byte, 1024)
	for cnt, errReading := file.Read(bytes); cnt > 0; cnt, errReading = file.Read(bytes) {
		check(errReading)
		if (cnt < 1024) {
			chksum.Write(bytes[0:cnt])
		} else {
			chksum.Write(bytes)
		}
	}
	file.Close()
	return chksum
}
// Comment out this function on production servers:
func initialize() bool {
	//
	//	Make sure we're in the right folder
	//
	if (len(cssDir) > 0) && (len(sdkDir) > 0)  && (cssDir != sdkDir) && fileExists("AV-Stylesheet.css") {
		files, err := ioutil.ReadDir(cssDir)
		if (err != nil) {
			return false
		}

		//		if err != nil {
		for _, f := range files {
			name := f.Name()
			if (len(name) <= 3) || (name[0]|0x20 != 'a') || (name[1]|0x20 != 'v') || (name[2] != '-') {
				path := cssDir + "/" + name
				if (isFileType(path, ".avspec")) {
					os.Remove(path)
					pathcss := replaceFileType(path, ".css")
					if fileExists(pathcss) {
						os.Remove(pathcss)
					}
				}
			}
		}
		return true
	}
	return false
}
// Comment out this function on production servers:
func reset(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "<h3>Initializing stylesheet folder ...")

	if initialize() {
		io.WriteString(w, " done<h3>")
	} else {
		io.WriteString(w, " unexpected error<h3>")
	}
}
// Comment out this function on production servers:
func release(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "<h3>Creating Inventory ...<h3>")

	if len(sdkDir) > 0 {
		files, err := ioutil.ReadDir(sdkDir)
		check(err)

		bom, err := os.Create(sdkDir + "/AV-Inventory.bom")
		check(err)

//		if err != nil {
			for _, f := range files {
				name := f.Name()
				if (len(name) >= 4) && (name[0]|0x20 == 'a') && (name[1]|0x20 == 'v') && (name[2] == '-') {
					chksum := createMD5(sdkDir+"/"+name, true)
					fmt.Fprintf(bom, "%X\t%s\n", chksum.Sum(nil), name)
					io.WriteString(w, name+"<br>")
				}
			}
			bom.Close()
			io.WriteString(w, "<h3>done</h3>")

//		} else {
//			io.WriteString(w, " ERROR</h3>")
//		}
	}
}
func validate(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "<h3>Validating Inventory ...</h2>")

	if len(sdkDir) > 0 {
		bom, err := os.Open(sdkDir + "/AV-Inventory.bom")
		check(err)
		defer bom.Close()

		scanner := bufio.NewScanner(bom)
		for scanner.Scan() {
			parts := strings.Split(scanner.Text(), "\t")
			name := parts[1]
			valid := parts[0]
			chksum := createMD5(sdkDir + "/" + name, false)

			if chksum == nil {
				io.WriteString(w, "ERROR: " + name + ":\t(cannot open file)<br>")
			} else {
				astext := fmt.Sprintf("%X", chksum.Sum(nil))
				if (astext == valid) {
					io.WriteString(w, "valid: " + name + "<br>")
				} else {
					io.WriteString(w, "ERROR: " + name + ":\texpected:\t" + valid + "\tgot:\t" + astext + "<br>")
				}
			}
		}
		bom.Close()
	}
}
func slash(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "<h2>This is the Digital-AV web-server.</h2><br>" +
		"<h3>Version: 2017." + strings.Replace(Version, "#", "", 1) + "</h3><br>" +
		"See: <p>" +
			"<a href=\"http://avtext.org\">http://avtext.org</a><br>" +
			"<a href=\"http://Digital-AV.org\">http:///Digital-AV.org</a>" +
		"</p>")
}
func help(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "See: <p>" +
		"<a href=\"http://avtext.org/SDK\">http://avtext.org/SDK</a>" +
		"</p>")
}
func greekSpecificCSS(key uint16) string {
//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
//	. notation is for classes
//  # notation is for id'
	return "span.G" + strings.ToUpper(strconv.FormatInt(int64(key), 36)) +	// this will never have any effect, but is interesting test
	" { font-weight: bold; color: blue; }"
}
func hebrewSpecificCSS(key uint16) string {
	//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
	//	. notation is for classes
	//  # notation is for id'
	return "span.H" + strings.ToUpper(strconv.FormatInt(int64(key), 36)) +	// this will never have any effect, but is interesting test
	" { font-weight: bold; color: blue; }";
}
func englisHSpecificCSS(key uint16) string {
	//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
	//	. notation is for classes
	//  # notation is for id'
	return "span.K" + strings.ToUpper(strconv.FormatInt(int64(key), 0x10)) + " { font-weight: bold; color: blue; }";
}
func wordSpecificCSS(word uint16) string {
	key := uint16(word & 0x3FFF)

	switch word & 0xC000 {
	case 0x0000: return englisHSpecificCSS(key) + "\n"
	case 0x8000: return hebrewSpecificCSS(key) + "\n"
	case 0x4000: return greekSpecificCSS(key) + "\n"
	}
	return "\n"
}
func css(w http.ResponseWriter, r *http.Request) {
	path := strings.Split(r.URL.String(), "/css/")

	if (len(path) == 2) && (len(path[0]) == 0) {
		cssPath := cssDir + "/" + path[1]

		if fileExists(cssPath) {
			data, err := ioutil.ReadFile(cssPath)
			if err == nil {
				w.Header().Set("Content-Type", "text/css; charset=utf-8")
				response := string(data)
				io.WriteString(w, response)
				return
			}
		} else {
			last := strings.LastIndex(path[1], ".css")
			if last+4 == len(path[1]) {
				avSpec := strings.Replace(path[1], ".css", ".avspec", 1)
				avPath := cssDir + "/" + avSpec
				if fileExists(avPath) {
					avFile, errAvvData := os.Open(avPath)
					cssFile, errCssFile := os.Create(cssPath)

					check(errAvvData)
					check(errCssFile)

					w.Header().Set("Content-Type", "text/css; charset=utf-8")

					wordCnt := readUInt16(avFile);
					for i := uint16(0); i < wordCnt; i++ {
						cssLine := wordSpecificCSS(readUInt16(avFile))
						io.WriteString(w, cssLine)
						cssFile.WriteString(cssLine)
					}
					cssFile.Close()
					avFile.Close()
					return
				}
			}
		}
	}
	fourOfour(w, r)
}
func getBibleText(offset uint32) bibleText {
	if offset != 0xFFFFFFFF {
		CueBibleData(offset)
	}
	var record bibleText
	record.strongs = readStrongs(bibleData)
	record.verseIndex = readUInt16(bibleData)
	record.wordKey = readUInt16(bibleData)
	record.puncuation = readByte(bibleData)
	record.transCase = readByte(bibleData)
	record.pos = readUInt16(bibleData)
	return record
}
var widx uint16
func bookPreamble(name string, chapter byte, session string) string {
	widx = 0;
	bookChapter := bible[name].name + " " + strconv.FormatUint(uint64(chapter), 10);
	bkCh := (uint64(bible[name].bookNum) * 0x10) + uint64(chapter);
	encodedBookChapter := "AV" + strings.ToUpper(strconv.FormatUint(bkCh, 0x10))
	preamble := "<html><head><title>" + bookChapter + "</title>"
	if (len(cssDir) > 0) {
		preamble += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/AV-Baseline.css\" media=\"screen\" />"
		if (len(session) > 0) {
			preamble += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/" + session + ".css\" media=\"screen\" />"
		}
	}
	preamble += "</head><body id=\"" + encodedBookChapter + "\" version=\"" + Version + "\">"
	return preamble
}
func bookPostamble() string {

	return "</body></html>"
}
func showBook(book string, w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html; charset=utf-8")
	spec := strings.ToLower(r.URL.String())

	parameters := strings.Split(spec, "?")
	stylesheet := ""

	if (len(parameters) == 2) {
		spec = parameters[0];
		session := parameters[1]
		anchors := strings.Split(spec, "#")
		if (len(anchors) >= 2) {
			session = anchors[0]
		} else {
			anchors := strings.Split(spec, "#")
			if (len(anchors) >= 2) {
				spec = anchors[0]
			}
		}
		stylesheet = getStylesheet(session)
	}
	parts := strings.Split(spec, "/")
	chapter := byte(1)

	if len(parts) >= 3 {
		c, err := strconv.Atoi(parts[2])
		if err != nil {
			c = 1
		}
		chapter = byte(c)
	}
	bk := bible[book]
	offset := getChapterIndex(bk, chapter)
	io.WriteString(w, bookPreamble(book, chapter, stylesheet))
	stop := false
	for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
		word := getWord(record, bk, true)
		io.WriteString(w, word)
		stop = (record.transCase & EndOfChapter == EndOfChapter)
	}
	io.WriteString(w, bookPostamble())
}
func getChapterIndex(bk book, chapter byte) uint32 {
	CueChapterIndex(bk.chapterIdx, chapter)
	return readUInt32(chapterIndex);
}
func getBooks(index *os.File) []book {
	bible := make([]book, 66)
	for n := byte(1); n <= 66; n++ {
		i := n-1;
		bible[i].chapterIdx = readUInt16(index)
		bible[i].bookNum = readByte(index)
		if bible[i].bookNum != n {
			return nil
		}
		bible[i].chapterCnt = readByte(index)
		extra := make([]byte, 32)
		x, err := index.Read(extra)
		check(err)
		if (x != 32) {
			return nil;
		}
		bible[i].name = ""
		idx := 0
		delimiter := 0;
		for k := 0; k < len(extra); k++ {
			switch(delimiter) {
			case 0:
				if (extra[k] == '/') {
					bible[i].name = string(extra[0:k:k])
					idx = k+1;
					delimiter ++;
				}
			case 1:
				if (extra[k] == ':') {
					bible[i].abbreviation = string(extra[idx:k:k])
					idx = k+1;
					delimiter ++;
				}
			case 2:
				if (extra[k] == '/') {
					bible[i].abbreviationMin = extra[idx] - '0';
					idx = k+1;
					delimiter ++;
				}
			case 3:
				if (extra[k] == ':') {
					bible[i].alternate = string(extra[idx:k:k])
					idx = k+1;
					delimiter ++;
				}
			case 4:
				if (extra[k] == byte(0)) {
					bible[i].alternateMin = extra[idx] - '0';
					break;
				}
			}
		}
	}
	return bible;
}
// AV-SDK treats 64-bit strong's array as a series of [4x] uit16 numbers
func readStrongs(file *os.File) []uint16 {
	quad := make([]uint16, 4)
	for i := 0; i < 4; i++ {
		quad[i] = readUInt16(file)
		if (quad[i] == 0xFFFF) {
			return nil;
		}
	}
	return quad;  // how did we get here ??? !!!!
}
// AV-SDK uses Little-Endian byte order to store data
func readUInt32(file *os.File) uint32 {
	quad := make([]byte, 4)
	n, err := file.Read(quad);
	check(err)

	if (n == 4) {
		result := (uint32(quad[3]) * 0x1000000)
		result += (uint32(quad[2]) * 0x10000);
		result += (uint32(quad[1]) * 0x100);
		result +=  uint32(quad[0]);
		return result;
	}
	return 0xFFFF;  // how did we get here ??? !!!!
}
// AV-SDK uses Big-Endian byte order on incoming hex-streams. just like every high-level language
func convUInt16(pair []byte) uint16 {
	if (len(pair) >= 2) {
		result := uint16(pair[0]) * 0x100
		result += uint16(pair[1]);
		return result;
	}
	return 0xFFFF;  // how did we get here ??? !!!!
}
// AV-SDK uses Little-Endian byte order to store data
func readUInt16(file *os.File) uint16 {
	pair := make([]byte, 2)
	n, err := file.Read(pair);
	check(err)

	if (n == 2) {
		result := uint16(pair[1]) * 0x100
		result += uint16(pair[0]);
		return result;
	}
	return 0xFFFF;  // how did we get here ??? !!!!
}
// AV-SDK uses Little-Endian byte order to store data
func writeUInt16(file *os.File, val uint16) {
	lo := byte(val / 0x100)
	hi := byte(val & 0x0FF)
	pair := make([]byte, 2)
	pair[1] = lo
	pair[0] = hi
	file.Write(pair)
}
func readByte(file *os.File) byte {
	singleton := make([]byte, 1)
	n, err := file.Read(singleton);
	check(err)

	if (n == 1) {
		return singleton[0];
	}
	return 0xFF;  // how did we get here ??? !!!!
}
func expandHyphens(input [] byte) []byte {
	output := make([]byte,24)

	hyphens := 0
	for k := 0; k < len(input); k++ {
		output[k+hyphens] = input[k] & 0x7F
		if input[k] & 0x80 == 0x80 {
			hyphens++
			output[k+hyphens] = '-'
		}
	}
	if (hyphens > 0) {
		cnt := len(input) + hyphens
		return output[0:cnt:cnt]
	}
	return input
}
func getLex(key uint16) string {
	find := key & uint16(0x3FFF);	// strip off capitolization-bits, if present
	if (find >= 1) {
		find-- // and make zero-based
		base := int(0);
		for n := uint16(1); n <= uint16(len(lex)); n++ { // this only works, because there are no holes in the map ([1:18]
			entries := lex[n]
			count := len(entries) / int(n+1)
			if (find < uint16(base+count)) {
				idx1 := int(find) - base;
				idx1 *= int(n + 1)
				idx2 := idx1 + int(n)
				word := entries[idx1:idx2:idx2]
				expanded := expandHyphens(word)
				return string(expanded)
			}
			base += count;
		}
	}
	return ""
}
func avCompareString(testStr string, lexStr string) bool {	// assume testStr is already lowercase
	size := len(lexStr)
	same := (len(testStr) == size)
	if (same) {
		for i := 0; i < size; i ++ {
			if (lexStr[i] & 0x7F) | 0x20 != testStr[i] { // compare w/o hyphen bit and or-in lowercase (lowercase, because mixed-case makes caps in lex legal
				return false;
			}
		}
	}
	return same;
}

func reverseLex(word string) uint16 {
	find := strings.ToLower(strings.Replace(word, "-", "", -1))
	size := len(find)
	apostrophe := strings.Split(find, "'")
	if (len(apostrophe) == 2) {
		if (len(apostrophe[1]) == 0) || (apostrophe[1] == "s") {
			find = apostrophe[0];
			size = len(find)
		}
	}
	key := uint16(1);
	for n := 1; n <= len(lex); n++ { // this only works, because there are no holes in the map ([1:18]
		entries := lex[uint16(n)]
		count := uint16(len(entries) / int(n+1))
		nextBase := uint16(key + count)

		if size == n {

			for idx1 := 0; key < nextBase; key++ {
				idx2 := idx1 + n
				word := entries[idx1:idx2:idx2]
				if avCompareString(find, string(word)) {
					return key
				}
				idx1 += (n+1)
			}
		} else { // skip
			key = nextBase;
		}
	}
	return 0
}
var currentVerse byte
var prev bibleText
func getWord(text bibleText, bk book, html bool) string {
	begVerse   := (text.transCase & VerseTransition)   == BeginingOfVerse
	endVerse   := (text.transCase & VerseTransition)   == EndOfVerse
	begChapter := (text.transCase & ChapterTransition) == BeginingOfChapter
	endChapter := (text.transCase & ChapterTransition) == EndOfChapter
//	begBook    := (text.transCase & BookTransition)    == BeginingOfBook
//	endBook    := (text.transCase & BookTransition)    == EndOfBook
	if begChapter {
		currentVerse = 1
	} else if begVerse {
		currentVerse ++
	}
	widx ++;
	word := getLex(text.wordKey & 0x3FFF)
	caps := text.wordKey & 0xC000
	if caps != 0 {
		if caps == 0x4000 {
			word = strings.ToUpper(word)
		}
		if caps == 0x8000 {
			first := strings.ToUpper(word[0:1])
			right := word[1:]
			word = first + right;
		}
	}
	if (html) {
		var delimiter string
		sstr := ""
		if bk.bookNum <= 39 {
			delimiter = "H"
		} else {
			delimiter = "G"
		}
		for s := 0; s < len(text.strongs); s++ {
			if text.strongs[s] != 0 {
				sstr += delimiter;
				sstr += strings.ToUpper(strconv.FormatInt(int64(text.strongs[s]), 36))
			} else {
				break;
			}
			delimiter = "_"
		}
		key := text.wordKey & 0x3FFF
		kstr := strings.ToUpper(strconv.FormatInt(int64(key),  0x10))
		wstr := strings.ToUpper(strconv.FormatInt(int64(widx), 36))
		word = "<span id=\"W" + wstr + "\" class=\"K" + kstr + "\" strongs=\"" + sstr + "\">" + word + "</span>"
	}
	if (begChapter) {
		if (text.puncuation&MODEitalics == MODEitalics) {
			word = "[" + word
		}
		if (text.puncuation&MODEparenthetical == MODEparenthetical) {
			word = "(" + word
		}
	} else {
		if (text.puncuation&MODEitalics == MODEitalics) && (prev.puncuation&MODEitalics != MODEitalics) {
			if html {
				word = "<em>" + word
			} else {
				word = "[" + word
			}
		}
		if (text.puncuation&MODEparenthetical == MODEparenthetical) && (prev.puncuation&MODEparenthetical != MODEparenthetical) {
			word = "(" + word
		}
	}
	if (text.puncuation & PUNCpossessive) != 0 {
		if (word[len(word)-1] != 's') {
			word += "'s"
		} else {
			word += "'"
		}
	}
	switch text.puncuation & PUNCclause {
	case PUNCexclamatory:	word += "!"
	case PUNCinterrogative:	word += "?"
	case PUNCdeclarative:	word += "."
	case PUNCdash:			word += "--"
	case PUNCsemicolon:		word += ";"
	case PUNCcomma:			word += ","
	case PUNCcolon:			word += ":"
	}
	if (!begChapter) {
		if (text.puncuation&MODEitalics != MODEitalics) && (prev.puncuation&MODEitalics == MODEitalics) {
			if html {
				word += "</em>"
			} else {
				word += "]"
			}
		}
		if (text.puncuation&MODEparenthetical != MODEparenthetical) && (prev.puncuation&MODEparenthetical == MODEparenthetical) {
			word += ")"
		}
	}
	if html {
		if begVerse {
			vstr := strconv.FormatUint(uint64(currentVerse), 10)
			word = "<span class=\"verse\" id=\"V" + vstr + "\"><span class=\"num\" id=\"N" + vstr + "\">" + vstr + " </span>" + word + " "
		} else if endVerse {
			word += "</span><br>"
		} else {
			word += " "
		}
	} else {
		if !endChapter {
			word += " "
		}
	}
	prev = text
	return word
}
var books []book;
var bible map[string]book
var lex map[uint16][]byte
var mux map[string]func(http.ResponseWriter, *http.Request)
var sdkDir string
var cssDir string
var chapterReady = false
var chapterIndex *os.File
func CueChapterIndex(bookPosition uint16, chapter byte) *os.File {
	if !chapterReady {
		idx, err := os.Open(sdkDir + "/AV-Chapter.IX4")
		check(err)
		chapterIndex = idx
		chapterReady = true;
	}
	position := int64(bookPosition);
	if chapter > 1 {
		position += int64(chapter-1)
	}
	chapterIndex.Seek(position*8, 0)
	return chapterIndex
}
var bibleReady = false
var bibleData *os.File
func CueBibleData(position uint32) *os.File {
	if !bibleReady {
		idx, err := os.Open(sdkDir + "/AV-Writ.DX8")
		check(err)
		bibleData = idx
		bibleReady = true;
	}
	bibleData.Seek(int64(position)*16, 0)
	return bibleData
}
func check(e error) {
	if e != nil {
		panic(e)
	}
}
func main() {	// Arguments SDK_DIR  CSS_DIR
	port := os.Getenv("PORT")

	if port == "" {
		port=":2121"
	} else {
		port=":"+port
	}
	server := http.Server{
		Addr:    port,
		Handler: &myHandler{},
	}
	if len(os.Args) >= 2 {
		sdkDir = os.Args[1]
		cssDir = sdkDir
	} else {
		sdkDir = "."
		cssDir = ""
	}
	if len(os.Args) >= 3 {
		cssDir = os.Args[2]
	}
	fbk, err := os.Open(sdkDir + "/AV-Book.IXI")
	check(err)
	books = getBooks(fbk)
	fbk.Close();
	lex = make(map[uint16][]byte)
	flx, err := os.Open(sdkDir + "/AV-Lexicon.VLT")
	check(err)
	for n := readUInt16(flx); n > 0 && n != 0xFFF; n = readUInt16(flx) {
		cnt := readUInt16(flx)
		list := make([]byte, int((n+1)*cnt))
		flx.Read(list)
		lex[n] = list;
	}
	flx.Close()
	mux = make(map[string]func(http.ResponseWriter, *http.Request))
	mux["/"] = slash
	mux["/validate"] = validate
	mux["/release"] = release	// comment out this line on production servers (this is the converse of validate
	mux["/help"] = help
	mux["/reset"] = reset
	bible = make(map[string]book)
	for _, b := range books[0:66] {
		lower := strings.ToLower(b.name);
		println("adding book support for: " + lower + " ...")
		bible[lower] = b;
	}
	initialize()
	server.ListenAndServe()
}
type myHandler struct{}
func (*myHandler) ServeHTTP(w http.ResponseWriter, r *http.Request) {
	if h, ok := mux[r.URL.String()]; ok {
		h(w, r)
		return
	}
	spec := strings.ToLower(r.URL.String())
	parts := strings.Split(spec, "/")
	if len(parts) >= 2 {
		if parts[1] == "css" {
			css(w, r)
			return
		}
		for _, b := range books[0:66] {
			lower := strings.ToLower(b.name)
			parts[1] = strings.Replace(parts[1], "%20", " ", -1 )
			stripped := strings.Replace(lower, " ", "", 99)
			if (parts[1] == lower || parts[1] == stripped) {
				showBook(lower, w, r)
				return
			}
		}
		for _, b := range books[0:66] {
			if parts[1] == strings.ToLower(b.abbreviation) {
				showBook(strings.ToLower(b.name), w, r)
				return
			}
		}
		for _, b := range books[0:66] {
			if b.abbreviationMin > 0 {
				if parts[1] == strings.ToLower(b.alternate) {
					showBook(strings.ToLower(b.name), w, r)
					return
				}
			}
		}
	}
	fourOfour(w, r)
}
