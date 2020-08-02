package main

//	Sample source code for Digital AV Edition -- Example based upon revision #i728
//	Copyright (c) 1996-2020 Kevin Wonus
import (
	"bufio"
	"crypto/md5"
	"fmt"
	"hash"
	"io"
	"io/ioutil"
	"net/http"
	"os"
	"strconv"
	"strings"

	"fivebitencoding"
)

const (
	PUNCclause        = 0xE0
	PUNCexclamatory   = 0x80
	PUNCinterrogative = 0xC0
	PUNCdeclarative   = 0xE0
	PUNCdash          = 0xA0
	PUNCsemicolon     = 0x20
	PUNCcomma         = 0x40
	PUNCcolon         = 0x60
	PUNCpossessive    = 0x10
	ENDparenthetical  = 0x0C
	MODEparenthetical = 0x04
	MODEitalics       = 0x02
	MODEjesus         = 0x01
)
const (
	EndBit            = 0x1 // (0b0001____)
	VerseTransition   = 0x3 // (0b0010____)
	BeginingOfVerse   = 0x2 // (0b0010____)
	EndOfVerse        = 0x3 // (0b0011____)
	ChapterTransition = 0x7 // (0b0110____)
	BeginingOfChapter = 0x6 // (0b0110____)
	EndOfChapter      = 0x7 // (0b0111____)
)
const (
	Version = "#I611"
)

type book struct {
	bookNum       byte
	chapterCnt    byte
	chapterIdx    uint16
	name          string
	abbreviations []string
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
	check(err) // otherwise, panic
	return false
}
func isFileType(file string, ext string) bool {
	info, err := os.Stat(file)
	if err != nil {
		return false
	} else if info.IsDir() {
		return strings.ToLower(ext) == "{dir}"
	}
	dot := strings.LastIndex(file, ".")
	if dot >= 0 {
		test := file[dot:]
		return strings.ToLower(test) == strings.ToLower(ext)
	}
	return false
}
func replaceFileType(file string, newext string) string {
	dot := strings.LastIndex(file, ".")
	if dot >= 1 {
		replacement := file[0:dot] + newext
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

			if assignment[0] == '$' {
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

/*
func decoratePN(pos uint16, pn byte) string {
	decoration := ""
	if pn != 0 {
		if pos&0x0F00 == 0x0100 { // this is a verb
			//			decoration = "<span class=\"pn\"><span class=\"nobr\">" + strconv.Itoa((int(pn)&0x70)/0x10) + "<br/>" + strconv.Itoa(int(pn)&0x7) + "</span></span>"
			decoration = "<sub>" + strconv.Itoa(int(pn)&0x7) + "</sub>"
		} else if pos&0x0F30 == 0x0030 {
			decoration = "<sub>" + strconv.Itoa(int(pn)&0x7) + "</sub>"
		}
	}
	return decoration
}
*/
func decoratePN(original string, modern string) string {
	decoration := ""
	endi := len(original) - 2

	if (len(modern) >= 3) && (modern[0:3] == "you") && original[0] == 't' {
		decoration = "<sub>t</sub>"
	} else if original == "art" || original == "wilt" {
		decoration = "<sub>t</sub>"
	} else if endi > 0 {
		end := original[endi:]
		if end == "th" {
			decoration = "<sub>3</sub>"
		} else if end == "st" {
			decoration = "<sub>t</sub>"
		}
	}
	return decoration
}
func decorateMD(original string, modern string) string {
	decoration := ""
	endi := len(original) - 2

	if (len(modern) >= 3) && (modern[0:3] == "you") && original[0] == 't' {
		decoration = "¹" // superscript-1 for singular
	} else if original == "art" || original == "wilt" {
		decoration = "¹" // superscript-1 for singular
	} else if endi > 0 {
		end := original[endi:]
		if end == "th" {
			decoration = "³" // superscript-3 for 3rd-person
		} else if end == "st" {
			decoration = "¹" // superscript-1 for singular
		}
	}
	return decoration
}
func fourOfour(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	w.WriteHeader(http.StatusNotFound)
	fmt.Fprintf(w, "404! Not found")
}
func createMD5(path string, missingIsFatal bool) hash.Hash {
	file, err := os.Open(path)
	if missingIsFatal {
		check(err)
	} else if err != nil {
		return nil
	}
	chksum := md5.New()
	bytes := make([]byte, 1024)
	for cnt, errReading := file.Read(bytes); cnt > 0; cnt, errReading = file.Read(bytes) {
		check(errReading)
		if cnt < 1024 {
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
	if (len(cssDir) > 0) && (len(sdkDir) > 0) && (cssDir != sdkDir) && fileExists("AV-Stylesheet.css") {
		files, err := ioutil.ReadDir(cssDir)
		if err != nil {
			return false
		}

		//		if err != nil {
		for _, f := range files {
			name := f.Name()
			if (len(name) <= 3) || (name[0]|0x20 != 'a') || (name[1]|0x20 != 'v') || (name[2] != '-') {
				path := cssDir + "/" + name
				if isFileType(path, ".avspec") {
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
/*
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
			av := (len(name) >= 4) && (name[0]|0x20 == 'a') && (name[1]|0x20 == 'v') && (name[2] == '-')
			avx := (len(name) >= 5) && (name[0]|0x20 == 'a') && (name[1]|0x20 == 'v') && (name[2]|0x20 == 'x') && (name[3] == '-')
			if av || avx {
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
*/
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
			chksum := createMD5(sdkDir+"/"+name, false)

			if chksum == nil {
				io.WriteString(w, "ERROR: "+name+":\t(cannot open file)<br>")
			} else {
				astext := fmt.Sprintf("%X", chksum.Sum(nil))
				if astext == valid {
					io.WriteString(w, "valid: "+name+"<br>")
				} else {
					io.WriteString(w, "ERROR: "+name+":\texpected:\t"+valid+"\tgot:\t"+astext+"<br>")
				}
			}
		}
		bom.Close()
	}
}
func slash(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w,
		"<h2>This is the AVX experimental bible by AV Text Ministries.</h2><br><h3>Version: 2019.JB21</h3><br>"+
			"See: <p><a href=\"https://avbible.net/news.html\">https://avbible.net/news.html</a>"+
			" for information about this exciting new development!<br></p>")
}
func help(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "text/html")
	io.WriteString(w, "See: <p>"+
		"<a href=\"http://avtext.org/SDK\">http://avtext.org/SDK</a>"+
		"</p>")
}
func greekSpecificCSS(key uint16) string {
	//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
	//	. notation is for classes
	//  # notation is for id'
	return "span.G" + strings.ToUpper(strconv.FormatInt(int64(key), 36)) + // this will never have any effect, but is interesting test
		" { font-weight: bold; color: blue; }"
}
func hebrewSpecificCSS(key uint16) string {
	//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
	//	. notation is for classes
	//  # notation is for id'
	return "span.H" + strings.ToUpper(strconv.FormatInt(int64(key), 36)) + // this will never have any effect, but is interesting test
		" { font-weight: bold; color: blue; }"
}
func englisHSpecificCSS(key uint16) string {
	//	This isn't sufficient for multi-strongs word segments, but it suffices for now (won't mark all matches until this is properly handled
	//	. notation is for classes
	//  # notation is for id'
	return "span.K" + strings.ToUpper(strconv.FormatInt(int64(key), 0x10)) + " { font-weight: bold; color: blue; }"
}
func wordSpecificCSS(word uint16) string {
	key := uint16(word & 0x3FFF)

	switch word & 0xC000 {
	case 0x0000:
		return englisHSpecificCSS(key) + "\n"
	case 0x8000:
		return hebrewSpecificCSS(key) + "\n"
	case 0x4000:
		return greekSpecificCSS(key) + "\n"
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

					wordCnt := readUInt16(avFile)
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

/*
func bookPreamble(name string, chapter byte, session string) string {
	widx = 0
	bookChapter := bible[name].name + " " + strconv.FormatUint(uint64(chapter), 10)
	bkCh := (uint64(bible[name].bookNum) * 0x10) + uint64(chapter)
	encodedBookChapter := "AV" + strings.ToUpper(strconv.FormatUint(bkCh, 0x10))

	preamble := "<!DOCTYPE html>\n<html><head><title>" + bookChapter + "</title>"
	preamble += "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script> <script>"
	preamble += "\n$('document').ready(function(){"
	preamble += "\n\tvar chrome = !!window.chrome;"
	preamble += "\n\tvar mac = (navigator.platform.toUpperCase().indexOf('MAC')>=0);"
	preamble += "\n\tif (parent != null && !(chrome || mac)) {"
	preamble += "\n\t\t\tparent.document.getElementById('av').style.height = (document['body'].offsetHeight+50) + 'px';"
	preamble += "\n});"
	preamble += "\n</script>"

	if len(cssDir) > 0 {
		preamble += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/AV-Baseline.css\" media=\"screen\" />"
		if len(session) > 0 {
			preamble += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/" + session + ".css\" media=\"screen\" />"
		}
	}
	preamble += "</head><body id=\"" + encodedBookChapter + "\" version=\"" + Version + "\">"
	return preamble
}
*/
func bookPostamble(book string, cstr string) string {

	link := "<br><br><a href=\"/side-by-side/" + book + "/" + cstr + "/\">View side-by-side verses</a>"

	return link + "</body></html>"
}
func getScriptedHeaderAndBodyPrefix(root string, book string, c int, iprev int, inext int, imax int, selected byte, session string) string {

	cmax := strconv.Itoa(imax)
	cstr := strconv.Itoa(c)

	prev := "/" + root + "/" + book + "/" + strconv.Itoa(iprev)
	next := "/" + root + "/" + book + "/" + strconv.Itoa(inext)
	last := "/" + root + "/" + book + "/" + cmax
	first := "/" + root + "/" + book + "/1"

	/*
		stylesheet := ""
		if len(session) > 0 {
			stylesheet = getStylesheet(session)
		}

		for i := 0; i < len(parameters); i++ {
			switch strings.ToLower(parameters[i]) {
			case "diff":
				diffs = true
				if i == 0 {
					session = ""
				}
			}
		}
		if len(session) > 0 {
			stylesheet = getStylesheet(session)
		}
	*/

	header := "<!DOCTYPE html>\n<html lang=\"en\">" +
		"<head>\n" +
		"<title>AV Text Ministries - " + book + " " + cmax + "</title>\n" +
		"<link rel=\"icon\" href=\"favicon.ico\" type=\"image/x-icon\">" +
		"<link rel=\"stylesheet\" type=\"text/css\" href=\"https://avbible.net/xform/side-by-side.css\">"

	if len(cssDir) > 0 {
		header += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/AV-Baseline.css\" media=\"screen\" />"
		if len(session) > 0 {
			header += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/css/" + session + ".css\" media=\"screen\" />"
		}
	}
	header += ("<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js\"></script>" +
		"<script src=\"https://avbible.net/xform/" + root + ".js\"></script>" +
		"<script>\n" +
		"document.domain = \"avbible.net\";" +
		"\n</script>" +
		"\n</head>")

	body := "\n<body><div class=\"inner\">" +
		"<select name=\"ComboBoxBook\" class=\"inner\" id=\"ComboBoxBook\" size=\"1\" onchange=\"BookChange()\">"

	for b := byte(1); b <= byte(66); b++ {
		bk := books[b-1]
		opt := "<option "
		if selected == b {
			opt += " selected=\"selected\""
		}
		opt += " value=\"" + strings.ToLower(bk.name) + ":" + strconv.Itoa(int(bk.chapterCnt)) + "\"> " + bk.name + "</option>\n"
		body += opt
	}
	body += ("\n</select>\n" +
		"<span class=\"vcr\">\n" +
		"<a id=\"CMIN\" href=\"" + first + "\">[1]</a>\n" +
		"<a href=\"" + prev + "\">&lt;&lt; </a> " +
		"<a id=\"chapter\" class=\"num\">" + cstr + "</a>" +
		"<a href=\"" + next + "\"> &gt;&gt;</a>" +
		"<a id=\"CMAX\" href=\"" + last + "\"> [" + cmax + "]</a>" +
		"</span>\n</div>\n" +
		"<div class=\"divTable\" scrolling=\"no\" frameborder=\"0\">\n" +
		"<div class=\"divTableBody\" scrolling=\"yes\" frameborder=\"0\">\n")

	return header + body
}
func showSideBySide(book string, chapter byte, w http.ResponseWriter, r *http.Request, showDiffs bool, session string) {

	current := bible[book]
	cm := int(current.chapterCnt)
	c := int(chapter)
	cstr := strconv.Itoa(c)

	cp := c - 1
	if cp < 1 {
		cp = 1
	}
	cn := c + 1
	if cn > cm {
		cn = cm
	}
	w.Header().Set("Content-Type", "text/html; charset=utf-8")
	io.WriteString(w, getScriptedHeaderAndBodyPrefix("side-by-side", book, c, cp, cn, cm, current.bookNum, session))

	rowBegin := "<div class=\"divTableRow\">\n<div class=\"divTableCell\">\n"
	rowEnd := "</div>\n</div>\n"

	cellSplit := "</div>\n<div class=\"divTableCell\">"

	left := rowBegin
	right := cellSplit

	offset := getChapterIndex(current, chapter)
	stop := false
	v := byte(0)
	body := ""
	wasParen := false
	for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
		eov := ((record.transCase & EndOfVerse) == EndOfVerse)
		lw, _ := getLex(record.wordKey, true)
		rw, _ := getLex(record.wordKey, false)

		lww := getWord(record, true, current, true, false, true, wasParen)
		rww := getWord(record, false, current, true, false, false, wasParen)
		diff := (lw != rw)
		if diff && showDiffs {
			lrep := "><span class=\"diff\">" + lw + "</span><"
			rrep := "><span class=\"diff\">" + rw + "</span><"

			lww = strings.Replace(lww, ">"+lw+"<", lrep, 1)
			rww = strings.Replace(rww, ">"+rw+"<", rrep, 1)
		}
		left += lww
		right += rww

		if strings.Contains(lww, ")") {
			wasParen = false
		} else if strings.Contains(lww, "(") {
			wasParen = true
		}
		if eov {
			v++
			if (current.bookNum == 66) && (chapter == 22) && (v == 20) {
				stop = true // bug at end-of-bible: fix it here
			} else {
				stop = (record.transCase&EndOfChapter == EndOfChapter)
			}
			body += left
			body += right
			body += rowEnd

			left = rowBegin
			right = cellSplit
		}
	}
	body += rowBegin
	body += "<br><a href=\"/avx/" + book + "/" + cstr + "/\">View AVX only</a>"
	body += cellSplit
	body += "<br><a href=\"/kjv/" + book + "/" + cstr + "/\">View KJV only</a>"
	body += rowEnd

	body += "\n</div>\n</div>\n</body>\n</html>"
	io.WriteString(w, body)
}

func showBook(book string, chapter byte, verse byte, w http.ResponseWriter, r *http.Request, session string, avx bool) {
	w.Header().Set("Content-Type", "text/html; charset=utf-8")

	parameters := strings.Split(session, "&")
	if len(parameters) >= 2 {
		session = parameters[0] // peel off the session parameter
	}

	bk := bible[book]
	current := bible[book]
	cm := int(current.chapterCnt)
	c := int(chapter)
	cstr := strconv.Itoa(c)

	cp := c - 1
	if cp < 1 {
		cp = 1
	}
	cn := c + 1
	if cn > cm {
		cn = cm
	}
	preamble := ""
	if avx {
		preamble = getScriptedHeaderAndBodyPrefix("avx", book, c, cp, cn, cm, current.bookNum, session)
	} else {
		preamble = getScriptedHeaderAndBodyPrefix("kjv", book, c, cp, cn, cm, current.bookNum, session)
	}
	io.WriteString(w, preamble)

	offset := getChapterIndex(bk, chapter)
	stop := false
	v := byte(0)
	wasParen := false
	for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
		eov := ((record.transCase & EndOfVerse) == EndOfVerse)
		//		bov := ((record.transCase & BeginingOfVerse) == BeginingOfVerse)
		word := getWord(record, avx, bk, true, false, true, wasParen)
		io.WriteString(w, word)

		if strings.Contains(word, ")") {
			wasParen = false
		} else if strings.Contains(word, "(") {
			wasParen = true
		}
		if eov {
			v++
			if (bk.bookNum == 66) && (chapter == 22) && (v == 20) {
				stop = true // bug at end-of-bible: fix it here
			} else {
				stop = (record.transCase&EndOfChapter == EndOfChapter)
			}
		}
	}
	io.WriteString(w, bookPostamble(book, cstr))
}
func showMarkdown(book string, chapter byte, verse byte, w http.ResponseWriter, r *http.Request, session string, avx bool) {
	w.Header().Set("Content-Type", "text/x-markdown; charset=utf-8")

	parameters := strings.Split(session, "&")
	if len(parameters) >= 2 {
		session = parameters[0] // peel off the session parameter
	}

	bk := bible[book]
	current := bible[book]
	cm := int(current.chapterCnt)
	c := int(chapter)
	cstr := strconv.Itoa(c)

	cp := c - 1
	if cp < 1 {
		cp = 1
	}
	cn := c + 1
	if cn > cm {
		cn = cm
	}
	preamble := "# " + book + " " + cstr
	if avx {
		preamble += " (avx)"
	} else {
		preamble += " (kjv)"
	}
	io.WriteString(w, preamble+"  \n1.\t")

	offset := getChapterIndex(bk, chapter)
	stop := false
	v := byte(0)
	wasParen := false
	for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
		eov := ((record.transCase & EndOfVerse) == EndOfVerse)
		word := getWord(record, avx, bk, false, true, true, wasParen)
		io.WriteString(w, word)

		if strings.Contains(word, ")") {
			wasParen = false
		} else if strings.Contains(word, "(") {
			wasParen = true
		}
		if eov {
			v++
			if (bk.bookNum == 66) && (chapter == 22) && (v == 20) {
				stop = true // bug at end-of-bible: fix it here
			} else {
				stop = (record.transCase&EndOfChapter == EndOfChapter)
			}
			eoc := ((record.transCase & EndOfChapter) == EndOfChapter)
			if eoc {
				eov = false
			}
			if eov {
				vstr := strconv.Itoa(int(v + 1))
				io.WriteString(w, "  \n"+vstr+".\t")
			}
		}
	}
}
func saveMarkdown(w http.ResponseWriter, book string, chapter byte, avx bool) bool {
	w.Header().Set("Content-Type", "text/html; charset=utf-8")

	path := "./export/"
	if avx {
		path += "avx/"
	} else {
		path += "kjv/"
	}
	bk := bible[book]
	b := int(bk.bookNum)
	bstr := strconv.Itoa(b)
	path += bstr
	os.MkdirAll(path, 0775)
	path += "/"

	c := int(chapter)
	cstr := strconv.Itoa(c)
	output := path + cstr + ".md"
	file, err := os.Create(output)

	if err != nil {
		io.WriteString(w, err.Error())
		return false
	} else {
		preamble := "# " + book + " " + cstr
		if avx {
			preamble += " (avx)"
		} else {
			preamble += " (kjv)"
		}
		file.WriteString(preamble + "  \n1.\t")

		offset := getChapterIndex(bk, chapter)
		stop := false
		v := byte(0)
		wasParen := false
		for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
			eov := ((record.transCase & EndOfVerse) == EndOfVerse)
			word := getWord(record, avx, bk, false, true, true, wasParen)
			file.WriteString(word)

			if strings.Contains(word, ")") {
				wasParen = false
			} else if strings.Contains(word, "(") {
				wasParen = true
			}
			if eov {
				v++
				if (bk.bookNum == 66) && (chapter == 22) && (v == 20) {
					stop = true // bug at end-of-bible: fix it here
				} else {
					stop = (record.transCase&EndOfChapter == EndOfChapter)
				}
				eoc := ((record.transCase & EndOfChapter) == EndOfChapter)
				if eoc {
					eov = false
				}
				if eov {
					vstr := strconv.Itoa(int(v + 1))
					file.WriteString("  \n" + vstr + ".\t")
				}
			}
		}
		file.Close()
		io.WriteString(w, "okay: "+output+"<br>")
	}
	return true
}
func showText(book string, chapter byte, verse byte, w http.ResponseWriter, r *http.Request, session string, avx bool) {
	w.Header().Set("Content-Type", "text/plain; charset=utf-8")

	parameters := strings.Split(session, "&")
	if len(parameters) >= 2 {
		session = parameters[0] // peel off the session parameter
	}

	bk := bible[book]
	current := bible[book]
	cm := int(current.chapterCnt)
	c := int(chapter)
	cstr := strconv.Itoa(c)

	cp := c - 1
	if cp < 1 {
		cp = 1
	}
	cn := c + 1
	if cn > cm {
		cn = cm
	}
	preamble := book + " " + cstr
	if avx {
		preamble += " (avx)"
	} else {
		preamble += " (kjv)"
	}
	io.WriteString(w, preamble+"\n1\t")

	offset := getChapterIndex(bk, chapter)
	stop := false
	v := byte(0)
	wasParen := false
	for record := getBibleText(offset); (record.wordKey != 0xFFFF) && !stop; record = getBibleText(0xFFFFFFFF) {
		eov := ((record.transCase & EndOfVerse) == EndOfVerse)
		word := getWord(record, avx, bk, false, false, true, wasParen)
		io.WriteString(w, word)

		if strings.Contains(word, ")") {
			wasParen = false
		} else if strings.Contains(word, "(") {
			wasParen = true
		}
		if eov {
			v++
			if (bk.bookNum == 66) && (chapter == 22) && (v == 20) {
				stop = true // bug at end-of-bible: fix it here
			} else {
				stop = (record.transCase&EndOfChapter == EndOfChapter)
			}
			eoc := ((record.transCase & EndOfChapter) == EndOfChapter)
			if eoc {
				eov = false
			}
			if eov {
				vstr := strconv.Itoa(int(v + 1))
				io.WriteString(w, "\n"+vstr+"\t")
			}
		}
	}
}
func getChapterIndex(bk book, chapter byte) uint32 {
	CueChapterIndex(bk.chapterIdx, chapter)
	return readUInt32(chapterIndex)
}
func getBooks(index *os.File) []book {
	bible := make([]book, 66)
	for n := byte(1); n <= 66; n++ {
		i := n - 1
		bible[i].bookNum = readByte(index)
		bible[i].chapterCnt = readByte(index)
		bible[i].chapterIdx = readUInt16(index)
		bkname := make([]byte, 16)
		x1, err1 := index.Read(bkname)
		check(err1)
		if x1 == 16 {
			c := 0
			for c = 0; c < 16; c++ {
				if bkname[c] == byte(0) {
					break
				}
			}
			if c < 16 {
				bible[i].name = strings.TrimSpace(string(bkname[:c]))
			} else {
				bible[i].name = strings.TrimSpace(string(bkname[:]))
			}
		} else {
			bible[i].name = ""
		}
		bkabbr := make([]byte, 12)
		x2, err2 := index.Read(bkabbr)
		check(err2)
		if x2 == 12 {
			c := 0
			for c = 0; c < 12; c++ {
				if bkabbr[c] == byte(0) {
					break
				}
			}
			abbr := ""
			if c < 12 {
				abbr = strings.TrimSpace(string(bkabbr[:c]))
			} else {
				abbr = strings.TrimSpace(string(bkabbr[:]))
			}
			bible[i].abbreviations = strings.Split(strings.TrimSpace(abbr), ",")
		} else {
			bible[i].abbreviations = make([]string, 0, 0)
		}
	}
	return bible
}

// AV-SDK treats 64-bit strong's array as a series of [4x] uit16 numbers
func readStrongs(file *os.File) []uint16 {
	quad := make([]uint16, 4)
	for i := 0; i < 4; i++ {
		quad[i] = readUInt16(file)
		if quad[i] == 0xFFFF {
			return nil
		}
	}
	return quad // how did we get here ??? !!!!
}

// AV-SDK uses Little-Endian byte order to store data
func readUInt32(file *os.File) uint32 {
	quad := make([]byte, 4)
	n, err := file.Read(quad)
	check(err)

	if n == 4 {
		result := (uint32(quad[3]) * 0x1000000)
		result += (uint32(quad[2]) * 0x10000)
		result += (uint32(quad[1]) * 0x100)
		result += uint32(quad[0])
		return result
	}
	return 0xFFFF // how did we get here ??? !!!!
}

// AV-SDK uses Big-Endian byte order on incoming hex-streams. just like every high-level language
func convUInt16(pair []byte) uint16 {
	if len(pair) >= 2 {
		result := uint16(pair[0]) * 0x100
		result += uint16(pair[1])
		return result
	}
	return 0xFFFF // how did we get here ??? !!!!
}

// AV-SDK uses Little-Endian byte order to store data
func readUInt16(file *os.File) uint16 {
	pair := make([]byte, 2)
	n, err := file.Read(pair)
	check(err)

	if n == 2 {
		result := uint16(pair[1]) * 0x100
		result += uint16(pair[0])
		return result
	}
	return 0xFFFF // how did we get here ??? !!!!
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
	n, err := file.Read(singleton)
	check(err)

	if n == 1 {
		return singleton[0]
	}
	return 0xFF // how did we get here ??? !!!!
}
func expandHyphens(input []byte) []byte {
	output := make([]byte, 24)

	hyphens := 0
	for k := 0; k < len(input); k++ {
		output[k+hyphens] = input[k] & 0x7F
		if input[k]&0x80 == 0x80 {
			hyphens++
			output[k+hyphens] = '-'
		}
	}
	if hyphens > 0 {
		cnt := len(input) + hyphens
		return output[0:cnt:cnt]
	}
	return input
}
func getLex(key uint16, isAVX bool) (string, bool) {
	if (key >= 1) && (key <= maxKey) {
		find := key & uint16(0x3FFF) // strip off capitolization-bits, if present
		modern := avx[find]
		legacy := kjv[find]

		if isAVX {
			return modern, (modern != legacy)
		}
		return legacy, (modern != legacy)
	}
	return "", true
}
func avCompareString(testStr string, lexStr string) bool { // assume testStr is already lowercase
	size := len(lexStr)
	same := (len(testStr) == size)
	if same {
		for i := 0; i < size; i++ {
			if (lexStr[i]&0x7F)|0x20 != testStr[i] { // compare w/o hyphen bit and or-in lowercase (lowercase, because mixed-case makes caps in lex legal
				return false
			}
		}
	}
	return same
}

func reverseLex(word string) uint16 {
	find := strings.ToLower(strings.Replace(word, "-", "", -1))
	apostrophe := strings.Split(find, "'")
	if len(apostrophe) == 2 {
		if (len(apostrophe[1]) == 0) || (apostrophe[1] == "s") {
			find = apostrophe[0]
		}
	}
	for n := 1; n <= len(lex); n++ { // this only works, because there are no holes in the map ([1:18]
		key := uint16(1)
		if find == lex[key] {
			return key
		}
	}
	return 0
}

var currentVerse byte

func getWord(text bibleText, modern bool, bk book, html bool, md bool, inc bool, wasParenthetical bool) string {
	begVerse := (text.transCase & VerseTransition) == BeginingOfVerse
	endVerse := (text.transCase & VerseTransition) == EndOfVerse
	begChapter := (text.transCase & ChapterTransition) == BeginingOfChapter
	endChapter := (text.transCase & ChapterTransition) == EndOfChapter
	//	begBook    := (text.transCase & BookTransition)    == BeginingOfBook
	//	endBook    := (text.transCase & BookTransition)    == EndOfBook
	if begChapter {
		currentVerse = 1
	} else if begVerse && inc { // for side-by-side view, do NOT increment on right-side passage
		currentVerse++
	}
	idSuffix := ""
	if modern {
		idSuffix = "x"
	}
	widx++
	key := text.wordKey & 0x3FFF
	word, _ := getLex(key, modern)
	caps := text.wordKey & 0xC000
	if caps != 0 {
		if caps == 0x4000 {
			word = strings.ToUpper(word)
		}
		if caps == 0x8000 {
			first := strings.ToUpper(word[0:1])
			right := word[1:]
			word = first + right
		}
	}
	if html {
		var delimiter string
		sstr := ""
		if bk.bookNum <= 39 {
			delimiter = "H"
		} else {
			delimiter = "G"
		}
		for s := 0; s < len(text.strongs); s++ {
			if text.strongs[s] != 0 {
				sstr += delimiter
				sstr += strings.ToUpper(strconv.FormatInt(int64(text.strongs[s]), 36))
			} else {
				break
			}
			delimiter = "_"
		}
		key := text.wordKey & 0x3FFF
		kstr := strings.ToUpper(strconv.FormatInt(int64(key), 0x10))
		wstr := strings.ToUpper(strconv.FormatInt(int64(widx), 36))

		sub := decor[key]
		if len(sub) > 0 && !modern {
			sub = ""
		}
		word = "<span id=\"W" + wstr + idSuffix + "\" class=\"K" + kstr + "\" strongs=\"" + sstr + "\">" + word + sub + "</span>"
	} else if md && modern {
		sub := decorMD[key]
		if len(sub) > 0 && !inc {
			sub = ""
		}
		word += sub
	}
	if begChapter {
		if text.puncuation&MODEitalics == MODEitalics {
			word = "[" + word
		}
		if text.puncuation&MODEparenthetical == MODEparenthetical {
			word = "(" + word
		}
	} else {
		if text.puncuation&MODEitalics == MODEitalics {
			if html {
				word = "<em>" + word + "</em>"
			} else if md {
				word = "_" + word + "_"
			} else {
				word = "[" + word + "]"
			}
		}
		if (text.puncuation&MODEparenthetical == MODEparenthetical) && !wasParenthetical {
			word = "(" + word
		}
	}
	if (text.puncuation & PUNCpossessive) != 0 {
		if word[len(word)-1] != 's' {
			word += "'s"
		} else {
			word += "'"
		}
	}
	switch text.puncuation & PUNCclause {
	case PUNCexclamatory:
		word += "!"
	case PUNCinterrogative:
		word += "?"
	case PUNCdeclarative:
		word += "."
	case PUNCdash:
		word += "--"
	case PUNCsemicolon:
		word += ";"
	case PUNCcomma:
		word += ","
	case PUNCcolon:
		word += ":"
	}
	if !begChapter {
		if text.puncuation&ENDparenthetical == ENDparenthetical {
			word += ")"
		}
	}
	if html {
		if begVerse {
			vstr := strconv.FormatUint(uint64(currentVerse), 10)
			word = "<span class=\"verse\" id=\"V" + vstr + idSuffix + "\"><span class=\"num\" id=\"N" + vstr + idSuffix + "\">" + vstr + " </span>" + word + " "
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
	return word
}

var books []book
var bible map[string]book
var maxKey uint16
var lex map[uint16]string
var avx map[uint16]string
var kjv map[uint16]string
var decor map[uint16]string
var decorMD map[uint16]string
var router map[string]func(http.ResponseWriter, *http.Request)
var sdkDir string
var cssDir string
var chapterReady = false
var chapterIndex *os.File

func CueChapterIndex(bookPosition uint16, chapter byte) *os.File {
	if !chapterReady {
		idx, err := os.Open(sdkDir + "/AV-Chapter.ix2")
		check(err)
		chapterIndex = idx
		chapterReady = true
	}
	position := int64(bookPosition)
	if chapter > 1 {
		position += int64(chapter - 1)
	}
	chapterIndex.Seek(position*8, 0)
	return chapterIndex
}

var bibleReady = false
var bibleData *os.File

func CueBibleData(position uint32) *os.File {
	if !bibleReady {
		idx, err := os.Open(sdkDir + "/AV-Writ.dx4")
		check(err)
		bibleData = idx
		bibleReady = true
	}
	bibleData.Seek(int64(position)*16, 0)
	return bibleData
}
func check(e error) {
	if e != nil {
		panic(e)
	}
}
func main() { // Arguments PORT  CSS_DIR
	port := os.Getenv("PORT")
	sdk := os.Getenv("AVSDK")

	if port == "" {
		port = ":2020"
	} else {
		port = ":" + port
	}
	sdkDir = "."
	cssDir = ""
	if sdk != "" {
		sdk = sdkDir
		cssDir = sdkDir
	}
	if len(os.Args) >= 2 {
		port = ":" + os.Args[1]
		if len(os.Args) >= 3 {
			cssDir = os.Args[2]
		}
	}
	fbk, err := os.Open(sdkDir + "/AV-Book.ix8")
	check(err)
	books = getBooks(fbk)
	fbk.Close()
	maxKey = 0
	lex = make(map[uint16]string)
	avx = make(map[uint16]string)
	kjv = make(map[uint16]string)
	decor = make(map[uint16]string)
	decorMD = make(map[uint16]string)
	flx, err := os.Open(sdkDir + "/AV-Lexicon.dxi")
	check(err)

	rawSearch := make([]uint16, 6, 6)
	rawDisplay := make([]uint16, 9, 9)
	rawModern := make([]uint16, 9, 9)

	key := uint16(0)
	for key = uint16(1); true; key++ {
		cnt := uint16(0)
		segment := uint16(0)
		for segment = readUInt16(flx); true; segment = readUInt16(flx) {
			if segment == 0 {
				goto done
			}
			rawSearch[cnt] = segment
			cnt++
			if segment&0x8000 == 0 { // not a continuation segment
				break
			}
		}
		lex[key] = fivebitencoding.Decode(rawSearch)
		diff := false
		cnt = 0
		for segment = readUInt16(flx); true; segment = readUInt16(flx) {
			if segment == 0 {
				break
			}
			diff = true
			rawDisplay[cnt] = segment
			cnt++
			if segment&0x8000 == 0 { // not a continuation segment
				break
			}
		}
		word := ""
		if diff {
			word = fivebitencoding.Decode(rawDisplay)
		} else {
			word = lex[key]
		}
		kjv[key] = word
		diff = false
		cnt = 0
		for segment = readUInt16(flx); true; segment = readUInt16(flx) {
			if segment == 0 {
				break
			}
			diff = true
			rawModern[cnt] = segment
			cnt++
			if segment&0x8000 == 0 { // not a continuation segment
				break
			}
		}
		modern := ""
		if diff {
			modern = fivebitencoding.Decode(rawModern)
		} else {
			modern = word
		}
		avx[key] = modern

		/*entities :=*/
		readUInt16(flx)

		cnt = readUInt16(flx)
		for p := uint16(0); p < cnt; p++ {
			pos := readUInt32(flx) // read and ignore POS segments
			fivebitencoding.DecodePOS(pos)
		}

		if diff {
			decor[key] = decoratePN(word, modern)
			decorMD[key] = decorateMD(word, modern)
		}
		key++
	}
done:
	maxKey = key - 1
	flx.Close()
	router = make(map[string]func(http.ResponseWriter, *http.Request))
	router["/"] = slash
	router["/validate"] = validate
	//	router["/release"] = release // comment out this line on production servers (this is the converse of validate
	router["/help"] = help
	router["/reset"] = reset
	bible = make(map[string]book)
	for _, b := range books[0:66] {
		lower := strings.ToLower(b.name)
		println("adding book support for: " + lower + " ...")
		bible[lower] = b
	}
	initialize()

	mux := http.NewServeMux()
	mux.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		if h, ok := router[r.URL.String()]; ok {
			h(w, r)
			return
		}
		book := ""
		chapter := byte(1)
		verse := byte(0) // anchor is not available, but allow highlighting with a redundant parameter: for avx/john/1/1/5/?123#V5
		params := ""
		unformatted := strings.ToLower(r.URL.String())
		parts := strings.Split(unformatted, "/")

		base := 0
		if parts[0] == "" {
			base++
		}
		if len(parts) <= base {
			fourOfour(w, r)
			return
		}
		isDual := (len(parts) >= base+2) && ((strings.ToLower(parts[base]) == "avx+av") || (strings.ToLower(parts[base]) == "side-by-side"))
		isText := (len(parts) >= base+2) && ((strings.ToLower(parts[base]) == "avx.text") || (strings.ToLower(parts[base]) == "av.text") || (strings.ToLower(parts[base]) == "kjv.text") || (strings.ToLower(parts[base]) == "text"))
		isMD := (len(parts) >= base+2) && ((strings.ToLower(parts[base]) == "avx.md") || (strings.ToLower(parts[base]) == "av.md") || (strings.ToLower(parts[base]) == "kjv.md") || (strings.ToLower(parts[base]) == "md"))
		isAVX := (len(parts) >= base+2) && ((strings.ToLower(parts[base]) == "avx") || (strings.ToLower(parts[base]) == "bible") || (strings.ToLower(parts[base]) == "avx.md") || (strings.ToLower(parts[base]) == "md") || (strings.ToLower(parts[base]) == "text"))
		isExport := (len(parts) >= base+2) && (strings.ToLower(parts[base]) == "export")

		if isDual || isAVX || isText || isMD || (len(parts) >= base+2) && ((strings.ToLower(parts[base]) == "kjv") || (strings.ToLower(parts[base]) == "av")) {
			base++
		}
		if isExport {
			for _, b := range books[0:66] {
				lower := strings.ToLower(b.name)
				for c := byte(1); c < b.chapterCnt; c++ {
					if (!saveMarkdown(w, lower, c, true)) || (!saveMarkdown(w, lower, c, false)) {
						break
					}
				}
			}
			return
		} else if (len(parts) > base) || isExport {
			book = parts[base]
			base++
			if book == "css" {
				css(w, r)
				return
			}
			if len(parts) > base {
				num := 0
				if strings.Contains(parts[base], "?") {
					subs := strings.Split(parts[base], "?")
					num, err = strconv.Atoi(subs[0])
					params = subs[1]
				} else {
					num, err = strconv.Atoi(parts[base])
				}
				if err != nil {
					num = 1
				}
				chapter = byte(num)
				base++
			}
			//	There might be a trailing verse spec ... like : john/1/1/5
			if len(parts) > base {
				num := 0
				if strings.Contains(parts[base], "?") {
					subs := strings.Split(parts[base], "?")
					num, err = strconv.Atoi(subs[0])
					params = subs[1]
				} else {
					num, err = strconv.Atoi(parts[base])
				}
				if err != nil {
					num = 0 // zero means: do not highlight
				}
				verse = byte(num)
				base++
			}
			//	There might be a trailing slash ... like : john/1/1/?123#V10
			if len(parts) > base {
				if strings.Contains(parts[base], "?") {
					subs := strings.Split(parts[base], "?")
					params = subs[1]
				}
			}
			for _, b := range books[0:66] {
				lower := strings.ToLower(b.name)
				book = strings.Replace(book, "%20", " ", -1)
				stripped := strings.Replace(lower, " ", "", 99)
				if book == lower || book == stripped {
					if isDual {
						showSideBySide(lower, chapter, w, r, true, params)
					} else if isMD {
						showMarkdown(lower, chapter, verse, w, r, params, isAVX)
					} else if isText {
						showText(lower, chapter, verse, w, r, params, isAVX)
					} else {
						showBook(lower, chapter, verse, w, r, params, isAVX)
					}
					return
				}
			}
			for _, b := range books[0:66] {
				if book == strings.ToLower(b.abbreviations[0]) {
					if isDual {
						showSideBySide(strings.ToLower(b.name), chapter, w, r, true, params)
					} else {
						showBook(strings.ToLower(b.name), chapter, verse, w, r, params, isAVX)
					}
					return
				}
			}
			for _, b := range books[0:66] {
				if book == strings.ToLower(b.abbreviations[0]) {
					if isDual {
						showSideBySide(strings.ToLower(b.name), chapter, w, r, true, params)
					} else {
						showBook(strings.ToLower(b.name), chapter, verse, w, r, params, isAVX)
					}
					return
				}
			}
		}
		fourOfour(w, r)
	})
	//	handler := cors.Default().Handler(mux)
	http.ListenAndServe(port, mux) //handler)
}
