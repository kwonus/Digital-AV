// > > > Generated-Code -- Header > > > //
// This file was partially code generated. Some edits to this module will be lost.
// Be sure NOT to add/change code within Generated-Code directives.
// For example, these comments are wrapped in a pair of Generated-Coded directives.
// < < < Generated-Code -- Header < < < //

// > > > Generated-Code -- Metadata > > > //
static AVXOOVLemmata_Rust_Edition    :u16 = 23108;
static AVXOOVLemmata_SDK_ZEdition    :u16 = 23107;

static AVXOOVLemmata_File: &'static str = "AV-Lemma-OOV.dxi";
static AVXOOVLemmata_RecordLen   :usize =        0;
static AVXOOVLemmata_RecordCnt   :usize =      771;
static AVXOOVLemmata_FileLen     :usize =     7754;
// < < < Generated-Code -- Metadata < < < //

// OOV: uint16                                           // from Digital-AV.pdf
static OOV_Marker :u16 = 0x8000;
static OOV_length :u16 = 0x0F00;
static OOV_index  :u16 = 0x000F;

struct AVXLemmaOOV{                                    // from Digital-AV.pdf
    key:  u16,
    word: &'static str,
}

// > > > Generated-Code -- Initialization > > > //
static oov_lemmata: [AVXLemmaOOV; 771] = [
	AVXLemmaOOV { key: 0x8301, word: "aid" },
	AVXLemmaOOV { key: 0x8302, word: "ail" },
	AVXLemmaOOV { key: 0x8303, word: "ape" },
	AVXLemmaOOV { key: 0x8304, word: "elm" },
	AVXLemmaOOV { key: 0x8305, word: "fen" },
	AVXLemmaOOV { key: 0x8306, word: "fin" },
	AVXLemmaOOV { key: 0x8307, word: "fix" },
	AVXLemmaOOV { key: 0x8308, word: "foe" },
	AVXLemmaOOV { key: 0x8309, word: "fry" },
	AVXLemmaOOV { key: 0x830A, word: "has" },
	AVXLemmaOOV { key: 0x830B, word: "hat" },
	AVXLemmaOOV { key: 0x830C, word: "mix" },
	AVXLemmaOOV { key: 0x830D, word: "mow" },
	AVXLemmaOOV { key: 0x830E, word: "nut" },
	AVXLemmaOOV { key: 0x830F, word: "pap" },
	AVXLemmaOOV { key: 0x8310, word: "rag" },
	AVXLemmaOOV { key: 0x8311, word: "rub" },
	AVXLemmaOOV { key: 0x8312, word: "tax" },
	AVXLemmaOOV { key: 0x8313, word: "tri" },
	AVXLemmaOOV { key: 0x8401, word: "ally" },
	AVXLemmaOOV { key: 0x8402, word: "aloe" },
	AVXLemmaOOV { key: 0x8403, word: "amid" },
	AVXLemmaOOV { key: 0x8404, word: "arch" },
	AVXLemmaOOV { key: 0x8405, word: "barb" },
	AVXLemmaOOV { key: 0x8406, word: "bean" },
	AVXLemmaOOV { key: 0x8407, word: "boll" },
	AVXLemmaOOV { key: 0x8408, word: "boss" },
	AVXLemmaOOV { key: 0x8409, word: "claw" },
	AVXLemmaOOV { key: 0x840A, word: "clip" },
	AVXLemmaOOV { key: 0x840B, word: "clod" },
	AVXLemmaOOV { key: 0x840C, word: "cope" },
	AVXLemmaOOV { key: 0x840D, word: "damn" },
	AVXLemmaOOV { key: 0x840E, word: "deem" },
	AVXLemmaOOV { key: 0x840F, word: "dram" },
	AVXLemmaOOV { key: 0x8410, word: "earn" },
	AVXLemmaOOV { key: 0x8411, word: "fork" },
	AVXLemmaOOV { key: 0x8412, word: "frog" },
	AVXLemmaOOV { key: 0x8413, word: "gape" },
	AVXLemmaOOV { key: 0x8414, word: "glit" },
	AVXLemmaOOV { key: 0x8415, word: "hood" },
	AVXLemmaOOV { key: 0x8416, word: "hose" },
	AVXLemmaOOV { key: 0x8417, word: "japh" },
	AVXLemmaOOV { key: 0x8418, word: "jest" },
	AVXLemmaOOV { key: 0x8419, word: "jump" },
	AVXLemmaOOV { key: 0x841A, word: "lane" },
	AVXLemmaOOV { key: 0x841B, word: "leek" },
	AVXLemmaOOV { key: 0x841C, word: "list" },
	AVXLemmaOOV { key: 0x841D, word: "load" },
	AVXLemmaOOV { key: 0x841E, word: "loin" },
	AVXLemmaOOV { key: 0x841F, word: "loop" },
	AVXLemmaOOV { key: 0x8420, word: "maim" },
	AVXLemmaOOV { key: 0x8421, word: "nave" },
	AVXLemmaOOV { key: 0x8422, word: "omit" },
	AVXLemmaOOV { key: 0x8423, word: "ouch" },
	AVXLemmaOOV { key: 0x8424, word: "pace" },
	AVXLemmaOOV { key: 0x8425, word: "pang" },
	AVXLemmaOOV { key: 0x8426, word: "pave" },
	AVXLemmaOOV { key: 0x8427, word: "peel" },
	AVXLemmaOOV { key: 0x8428, word: "plot" },
	AVXLemmaOOV { key: 0x8429, word: "poet" },
	AVXLemmaOOV { key: 0x842A, word: "pond" },
	AVXLemmaOOV { key: 0x842B, word: "puff" },
	AVXLemmaOOV { key: 0x842C, word: "rite" },
	AVXLemmaOOV { key: 0x842D, word: "ruby" },
	AVXLemmaOOV { key: 0x842E, word: "sear" },
	AVXLemmaOOV { key: 0x842F, word: "smit" },
	AVXLemmaOOV { key: 0x8430, word: "soak" },
	AVXLemmaOOV { key: 0x8431, word: "spew" },
	AVXLemmaOOV { key: 0x8432, word: "ston" },
	AVXLemmaOOV { key: 0x8433, word: "stud" },
	AVXLemmaOOV { key: 0x8434, word: "teat" },
	AVXLemmaOOV { key: 0x8435, word: "term" },
	AVXLemmaOOV { key: 0x8436, word: "toph" },
	AVXLemmaOOV { key: 0x8437, word: "trim" },
	AVXLemmaOOV { key: 0x8438, word: "twig" },
	AVXLemmaOOV { key: 0x8439, word: "twin" },
	AVXLemmaOOV { key: 0x843A, word: "valu" },
	AVXLemmaOOV { key: 0x843B, word: "wage" },
	AVXLemmaOOV { key: 0x843C, word: "wean" },
	AVXLemmaOOV { key: 0x843D, word: "weed" },
	AVXLemmaOOV { key: 0x843E, word: "wile" },
	AVXLemmaOOV { key: 0x843F, word: "wire" },
	AVXLemmaOOV { key: 0x8440, word: "zohe" },
	AVXLemmaOOV { key: 0x8501, word: "abate" },
	AVXLemmaOOV { key: 0x8502, word: "alame" },
	AVXLemmaOOV { key: 0x8503, word: "aleme" },
	AVXLemmaOOV { key: 0x8504, word: "amaze" },
	AVXLemmaOOV { key: 0x8505, word: "apron" },
	AVXLemmaOOV { key: 0x8506, word: "argue" },
	AVXLemmaOOV { key: 0x8507, word: "avail" },
	AVXLemmaOOV { key: 0x8508, word: "avite" },
	AVXLemmaOOV { key: 0x8509, word: "belie" },
	AVXLemmaOOV { key: 0x850A, word: "bench" },
	AVXLemmaOOV { key: 0x850B, word: "berry" },
	AVXLemmaOOV { key: 0x850C, word: "blain" },
	AVXLemmaOOV { key: 0x850D, word: "bleat" },
	AVXLemmaOOV { key: 0x850E, word: "bloom" },
	AVXLemmaOOV { key: 0x850F, word: "bowel" },
	AVXLemmaOOV { key: 0x8510, word: "brawl" },
	AVXLemmaOOV { key: 0x8511, word: "brief" },
	AVXLemmaOOV { key: 0x8512, word: "broil" },
	AVXLemmaOOV { key: 0x8513, word: "byway" },
	AVXLemmaOOV { key: 0x8514, word: "cabin" },
	AVXLemmaOOV { key: 0x8515, word: "carve" },
	AVXLemmaOOV { key: 0x8516, word: "chafe" },
	AVXLemmaOOV { key: 0x8517, word: "charm" },
	AVXLemmaOOV { key: 0x8518, word: "churn" },
	AVXLemmaOOV { key: 0x8519, word: "clout" },
	AVXLemmaOOV { key: 0x851A, word: "comer" },
	AVXLemmaOOV { key: 0x851B, word: "crash" },
	AVXLemmaOOV { key: 0x851C, word: "crave" },
	AVXLemmaOOV { key: 0x851D, word: "crisp" },
	AVXLemmaOOV { key: 0x851E, word: "crumb" },
	AVXLemmaOOV { key: 0x851F, word: "edifi" },
	AVXLemmaOOV { key: 0x8520, word: "endue" },
	AVXLemmaOOV { key: 0x8521, word: "erect" },
	AVXLemmaOOV { key: 0x8522, word: "erite" },
	AVXLemmaOOV { key: 0x8523, word: "fable" },
	AVXLemmaOOV { key: 0x8524, word: "fatle" },
	AVXLemmaOOV { key: 0x8525, word: "final" },
	AVXLemmaOOV { key: 0x8526, word: "fitch" },
	AVXLemmaOOV { key: 0x8527, word: "flake" },
	AVXLemmaOOV { key: 0x8528, word: "flank" },
	AVXLemmaOOV { key: 0x8529, word: "float" },
	AVXLemmaOOV { key: 0x852A, word: "forge" },
	AVXLemmaOOV { key: 0x852B, word: "frank" },
	AVXLemmaOOV { key: 0x852C, word: "gerah" },
	AVXLemmaOOV { key: 0x852D, word: "graft" },
	AVXLemmaOOV { key: 0x852E, word: "hater" },
	AVXLemmaOOV { key: 0x852F, word: "havoc" },
	AVXLemmaOOV { key: 0x8530, word: "hinge" },
	AVXLemmaOOV { key: 0x8531, word: "hoist" },
	AVXLemmaOOV { key: 0x8532, word: "lease" },
	AVXLemmaOOV { key: 0x8533, word: "ledge" },
	AVXLemmaOOV { key: 0x8534, word: "mason" },
	AVXLemmaOOV { key: 0x8535, word: "melon" },
	AVXLemmaOOV { key: 0x8536, word: "mince" },
	AVXLemmaOOV { key: 0x8537, word: "music" },
	AVXLemmaOOV { key: 0x8538, word: "neigh" },
	AVXLemmaOOV { key: 0x8539, word: "onion" },
	AVXLemmaOOV { key: 0x853A, word: "outgo" },
	AVXLemmaOOV { key: 0x853B, word: "paint" },
	AVXLemmaOOV { key: 0x853C, word: "parch" },
	AVXLemmaOOV { key: 0x853D, word: "party" },
	AVXLemmaOOV { key: 0x853E, word: "pilot" },
	AVXLemmaOOV { key: 0x853F, word: "piper" },
	AVXLemmaOOV { key: 0x8540, word: "plait" },
	AVXLemmaOOV { key: 0x8541, word: "plane" },
	AVXLemmaOOV { key: 0x8542, word: "plank" },
	AVXLemmaOOV { key: 0x8543, word: "prate" },
	AVXLemmaOOV { key: 0x8544, word: "prick" },
	AVXLemmaOOV { key: 0x8545, word: "quail" },
	AVXLemmaOOV { key: 0x8546, word: "revel" },
	AVXLemmaOOV { key: 0x8547, word: "ridge" },
	AVXLemmaOOV { key: 0x8548, word: "rifle" },
	AVXLemmaOOV { key: 0x8549, word: "rinse" },
	AVXLemmaOOV { key: 0x854A, word: "ripen" },
	AVXLemmaOOV { key: 0x854B, word: "rover" },
	AVXLemmaOOV { key: 0x854C, word: "rower" },
	AVXLemmaOOV { key: 0x854D, word: "scale" },
	AVXLemmaOOV { key: 0x854E, word: "scour" },
	AVXLemmaOOV { key: 0x854F, word: "shrub" },
	AVXLemmaOOV { key: 0x8550, word: "snort" },
	AVXLemmaOOV { key: 0x8551, word: "snuff" },
	AVXLemmaOOV { key: 0x8552, word: "spill" },
	AVXLemmaOOV { key: 0x8553, word: "spoke" },
	AVXLemmaOOV { key: 0x8554, word: "sprig" },
	AVXLemmaOOV { key: 0x8555, word: "stack" },
	AVXLemmaOOV { key: 0x8556, word: "stair" },
	AVXLemmaOOV { key: 0x8557, word: "stake" },
	AVXLemmaOOV { key: 0x8558, word: "stoic" },
	AVXLemmaOOV { key: 0x8559, word: "strew" },
	AVXLemmaOOV { key: 0x855A, word: "taber" },
	AVXLemmaOOV { key: 0x855B, word: "tache" },
	AVXLemmaOOV { key: 0x855C, word: "tarri" },
	AVXLemmaOOV { key: 0x855D, word: "tenon" },
	AVXLemmaOOV { key: 0x855E, word: "thong" },
	AVXLemmaOOV { key: 0x855F, word: "tutor" },
	AVXLemmaOOV { key: 0x8560, word: "twine" },
	AVXLemmaOOV { key: 0x8561, word: "urban" },
	AVXLemmaOOV { key: 0x8562, word: "valve" },
	AVXLemmaOOV { key: 0x8563, word: "waken" },
	AVXLemmaOOV { key: 0x8564, word: "waver" },
	AVXLemmaOOV { key: 0x8565, word: "weari" },
	AVXLemmaOOV { key: 0x8566, word: "whirl" },
	AVXLemmaOOV { key: 0x8567, word: "zuzim" },
	AVXLemmaOOV { key: 0x8601, word: "abided" },
	AVXLemmaOOV { key: 0x8602, word: "abieze" },
	AVXLemmaOOV { key: 0x8603, word: "abject" },
	AVXLemmaOOV { key: 0x8604, word: "abuser" },
	AVXLemmaOOV { key: 0x8605, word: "addict" },
	AVXLemmaOOV { key: 0x8606, word: "admire" },
	AVXLemmaOOV { key: 0x8607, word: "affair" },
	AVXLemmaOOV { key: 0x8608, word: "afford" },
	AVXLemmaOOV { key: 0x8609, word: "allege" },
	AVXLemmaOOV { key: 0x860A, word: "amount" },
	AVXLemmaOOV { key: 0x860B, word: "anakim" },
	AVXLemmaOOV { key: 0x860C, word: "apollo" },
	AVXLemmaOOV { key: 0x860D, word: "ardite" },
	AVXLemmaOOV { key: 0x860E, word: "arrive" },
	AVXLemmaOOV { key: 0x860F, word: "assign" },
	AVXLemmaOOV { key: 0x8610, word: "avouch" },
	AVXLemmaOOV { key: 0x8611, word: "azmave" },
	AVXLemmaOOV { key: 0x8612, word: "babble" },
	AVXLemmaOOV { key: 0x8613, word: "badger" },
	AVXLemmaOOV { key: 0x8614, word: "banish" },
	AVXLemmaOOV { key: 0x8615, word: "batter" },
	AVXLemmaOOV { key: 0x8616, word: "bearer" },
	AVXLemmaOOV { key: 0x8617, word: "beckon" },
	AVXLemmaOOV { key: 0x8618, word: "behead" },
	AVXLemmaOOV { key: 0x8619, word: "behoof" },
	AVXLemmaOOV { key: 0x861A, word: "belove" },
	AVXLemmaOOV { key: 0x861B, word: "berite" },
	AVXLemmaOOV { key: 0x861C, word: "billow" },
	AVXLemmaOOV { key: 0x861D, word: "bonnet" },
	AVXLemmaOOV { key: 0x861E, word: "bowman" },
	AVXLemmaOOV { key: 0x861F, word: "breech" },
	AVXLemmaOOV { key: 0x8620, word: "burier" },
	AVXLemmaOOV { key: 0x8621, word: "calker" },
	AVXLemmaOOV { key: 0x8622, word: "cellar" },
	AVXLemmaOOV { key: 0x8623, word: "collop" },
	AVXLemmaOOV { key: 0x8624, word: "confer" },
	AVXLemmaOOV { key: 0x8625, word: "cumber" },
	AVXLemmaOOV { key: 0x8626, word: "curdle" },
	AVXLemmaOOV { key: 0x8627, word: "dandle" },
	AVXLemmaOOV { key: 0x8628, word: "danite" },
	AVXLemmaOOV { key: 0x8629, word: "decent" },
	AVXLemmaOOV { key: 0x862A, word: "decide" },
	AVXLemmaOOV { key: 0x862B, word: "defame" },
	AVXLemmaOOV { key: 0x862C, word: "depose" },
	AVXLemmaOOV { key: 0x862D, word: "depute" },
	AVXLemmaOOV { key: 0x862E, word: "dismay" },
	AVXLemmaOOV { key: 0x862F, word: "eighti" },
	AVXLemmaOOV { key: 0x8630, word: "enable" },
	AVXLemmaOOV { key: 0x8631, word: "enfold" },
	AVXLemmaOOV { key: 0x8632, word: "engage" },
	AVXLemmaOOV { key: 0x8633, word: "engine" },
	AVXLemmaOOV { key: 0x8634, word: "exempt" },
	AVXLemmaOOV { key: 0x8635, word: "expect" },
	AVXLemmaOOV { key: 0x8636, word: "expire" },
	AVXLemmaOOV { key: 0x8637, word: "eyelid" },
	AVXLemmaOOV { key: 0x8638, word: "fanner" },
	AVXLemmaOOV { key: 0x8639, word: "fathom" },
	AVXLemmaOOV { key: 0x863A, word: "felloe" },
	AVXLemmaOOV { key: 0x863B, word: "fetter" },
	AVXLemmaOOV { key: 0x863C, word: "firkin" },
	AVXLemmaOOV { key: 0x863D, word: "forger" },
	AVXLemmaOOV { key: 0x863E, word: "freeze" },
	AVXLemmaOOV { key: 0x863F, word: "gallow" },
	AVXLemmaOOV { key: 0x8640, word: "garlic" },
	AVXLemmaOOV { key: 0x8641, word: "harken" },
	AVXLemmaOOV { key: 0x8642, word: "harper" },
	AVXLemmaOOV { key: 0x8643, word: "hazard" },
	AVXLemmaOOV { key: 0x8644, word: "indebt" },
	AVXLemmaOOV { key: 0x8645, word: "indite" },
	AVXLemmaOOV { key: 0x8646, word: "injure" },
	AVXLemmaOOV { key: 0x8647, word: "invite" },
	AVXLemmaOOV { key: 0x8648, word: "jambre" },
	AVXLemmaOOV { key: 0x8649, word: "jangle" },
	AVXLemmaOOV { key: 0x864A, word: "jesuit" },
	AVXLemmaOOV { key: 0x864B, word: "kernel" },
	AVXLemmaOOV { key: 0x864C, word: "kidney" },
	AVXLemmaOOV { key: 0x864D, word: "lancet" },
	AVXLemmaOOV { key: 0x864E, word: "libyan" },
	AVXLemmaOOV { key: 0x864F, word: "linger" },
	AVXLemmaOOV { key: 0x8650, word: "litter" },
	AVXLemmaOOV { key: 0x8651, word: "lydian" },
	AVXLemmaOOV { key: 0x8652, word: "madman" },
	AVXLemmaOOV { key: 0x8653, word: "mallow" },
	AVXLemmaOOV { key: 0x8654, word: "misper" },
	AVXLemmaOOV { key: 0x8655, word: "misuse" },
	AVXLemmaOOV { key: 0x8656, word: "motion" },
	AVXLemmaOOV { key: 0x8657, word: "mowing" },
	AVXLemmaOOV { key: 0x8658, word: "muster" },
	AVXLemmaOOV { key: 0x8659, word: "nazare" },
	AVXLemmaOOV { key: 0x865A, word: "nettle" },
	AVXLemmaOOV { key: 0x865B, word: "occupi" },
	AVXLemmaOOV { key: 0x865C, word: "orphan" },
	AVXLemmaOOV { key: 0x865D, word: "oznite" },
	AVXLemmaOOV { key: 0x865E, word: "parent" },
	AVXLemmaOOV { key: 0x865F, word: "planet" },
	AVXLemmaOOV { key: 0x8660, word: "plower" },
	AVXLemmaOOV { key: 0x8661, word: "pocher" },
	AVXLemmaOOV { key: 0x8662, word: "polish" },
	AVXLemmaOOV { key: 0x8663, word: "pommel" },
	AVXLemmaOOV { key: 0x8664, word: "prance" },
	AVXLemmaOOV { key: 0x8665, word: "public" },
	AVXLemmaOOV { key: 0x8666, word: "puhite" },
	AVXLemmaOOV { key: 0x8667, word: "quarry" },
	AVXLemmaOOV { key: 0x8668, word: "rafter" },
	AVXLemmaOOV { key: 0x8669, word: "raisin" },
	AVXLemmaOOV { key: 0x866A, word: "ramese" },
	AVXLemmaOOV { key: 0x866B, word: "rattle" },
	AVXLemmaOOV { key: 0x866C, word: "ravish" },
	AVXLemmaOOV { key: 0x866D, word: "reform" },
	AVXLemmaOOV { key: 0x866E, word: "repeat" },
	AVXLemmaOOV { key: 0x866F, word: "repute" },
	AVXLemmaOOV { key: 0x8670, word: "rumble" },
	AVXLemmaOOV { key: 0x8671, word: "sabean" },
	AVXLemmaOOV { key: 0x8672, word: "sailor" },
	AVXLemmaOOV { key: 0x8673, word: "sandal" },
	AVXLemmaOOV { key: 0x8674, word: "shiver" },
	AVXLemmaOOV { key: 0x8675, word: "shrine" },
	AVXLemmaOOV { key: 0x8676, word: "shrink" },
	AVXLemmaOOV { key: 0x8677, word: "slight" },
	AVXLemmaOOV { key: 0x8678, word: "sluice" },
	AVXLemmaOOV { key: 0x8679, word: "smiter" },
	AVXLemmaOOV { key: 0x867A, word: "sneeze" },
	AVXLemmaOOV { key: 0x867B, word: "solder" },
	AVXLemmaOOV { key: 0x867C, word: "sopher" },
	AVXLemmaOOV { key: 0x867D, word: "suborn" },
	AVXLemmaOOV { key: 0x867E, word: "subtle" },
	AVXLemmaOOV { key: 0x867F, word: "suburb" },
	AVXLemmaOOV { key: 0x8680, word: "suppli" },
	AVXLemmaOOV { key: 0x8681, word: "swerve" },
	AVXLemmaOOV { key: 0x8682, word: "syriac" },
	AVXLemmaOOV { key: 0x8683, word: "tablet" },
	AVXLemmaOOV { key: 0x8684, word: "tackle" },
	AVXLemmaOOV { key: 0x8685, word: "talker" },
	AVXLemmaOOV { key: 0x8686, word: "tavern" },
	AVXLemmaOOV { key: 0x8687, word: "tiding" },
	AVXLemmaOOV { key: 0x8688, word: "tinkle" },
	AVXLemmaOOV { key: 0x8689, word: "totter" },
	AVXLemmaOOV { key: 0x868A, word: "tumble" },
	AVXLemmaOOV { key: 0x868B, word: "ungird" },
	AVXLemmaOOV { key: 0x868C, word: "uprise" },
	AVXLemmaOOV { key: 0x868D, word: "verify" },
	AVXLemmaOOV { key: 0x868E, word: "wilful" },
	AVXLemmaOOV { key: 0x868F, word: "wimple" },
	AVXLemmaOOV { key: 0x8690, word: "winnow" },
	AVXLemmaOOV { key: 0x8691, word: "zorite" },
	AVXLemmaOOV { key: 0x8701, word: "accurse" },
	AVXLemmaOOV { key: 0x8702, word: "advance" },
	AVXLemmaOOV { key: 0x8703, word: "arelite" },
	AVXLemmaOOV { key: 0x8704, word: "armhole" },
	AVXLemmaOOV { key: 0x8705, word: "arodite" },
	AVXLemmaOOV { key: 0x8706, word: "ashtore" },
	AVXLemmaOOV { key: 0x8707, word: "assuage" },
	AVXLemmaOOV { key: 0x8708, word: "backbit" },
	AVXLemmaOOV { key: 0x8709, word: "baptise" },
	AVXLemmaOOV { key: 0x870A, word: "barjesu" },
	AVXLemmaOOV { key: 0x870B, word: "beriite" },
	AVXLemmaOOV { key: 0x870C, word: "bewitch" },
	AVXLemmaOOV { key: 0x870D, word: "boaster" },
	AVXLemmaOOV { key: 0x870E, word: "bringer" },
	AVXLemmaOOV { key: 0x870F, word: "broider" },
	AVXLemmaOOV { key: 0x8710, word: "bulwark" },
	AVXLemmaOOV { key: 0x8711, word: "burnish" },
	AVXLemmaOOV { key: 0x8712, word: "buttock" },
	AVXLemmaOOV { key: 0x8713, word: "caiapha" },
	AVXLemmaOOV { key: 0x8714, word: "carmite" },
	AVXLemmaOOV { key: 0x8715, word: "chaldee" },
	AVXLemmaOOV { key: 0x8716, word: "changer" },
	AVXLemmaOOV { key: 0x8717, word: "chapman" },
	AVXLemmaOOV { key: 0x8718, word: "chicken" },
	AVXLemmaOOV { key: 0x8719, word: "conform" },
	AVXLemmaOOV { key: 0x871A, word: "confuse" },
	AVXLemmaOOV { key: 0x871B, word: "congeal" },
	AVXLemmaOOV { key: 0x871C, word: "consort" },
	AVXLemmaOOV { key: 0x871D, word: "convict" },
	AVXLemmaOOV { key: 0x871E, word: "crackle" },
	AVXLemmaOOV { key: 0x871F, word: "dabbash" },
	AVXLemmaOOV { key: 0x8720, word: "deprive" },
	AVXLemmaOOV { key: 0x8721, word: "dinaite" },
	AVXLemmaOOV { key: 0x8722, word: "disdain" },
	AVXLemmaOOV { key: 0x8723, word: "dismiss" },
	AVXLemmaOOV { key: 0x8724, word: "disobey" },
	AVXLemmaOOV { key: 0x8725, word: "display" },
	AVXLemmaOOV { key: 0x8726, word: "dispose" },
	AVXLemmaOOV { key: 0x8727, word: "diviner" },
	AVXLemmaOOV { key: 0x8728, word: "downsit" },
	AVXLemmaOOV { key: 0x8729, word: "drinker" },
	AVXLemmaOOV { key: 0x872A, word: "dweller" },
	AVXLemmaOOV { key: 0x872B, word: "element" },
	AVXLemmaOOV { key: 0x872C, word: "elonite" },
	AVXLemmaOOV { key: 0x872D, word: "emptier" },
	AVXLemmaOOV { key: 0x872E, word: "enclose" },
	AVXLemmaOOV { key: 0x872F, word: "ensnare" },
	AVXLemmaOOV { key: 0x8730, word: "eranite" },
	AVXLemmaOOV { key: 0x8731, word: "espouse" },
	AVXLemmaOOV { key: 0x8732, word: "exactor" },
	AVXLemmaOOV { key: 0x8733, word: "expense" },
	AVXLemmaOOV { key: 0x8734, word: "exploit" },
	AVXLemmaOOV { key: 0x8735, word: "eyebrow" },
	AVXLemmaOOV { key: 0x8736, word: "falsify" },
	AVXLemmaOOV { key: 0x8737, word: "feather" },
	AVXLemmaOOV { key: 0x8738, word: "firepan" },
	AVXLemmaOOV { key: 0x8739, word: "flutter" },
	AVXLemmaOOV { key: 0x873A, word: "footman" },
	AVXLemmaOOV { key: 0x873B, word: "foresee" },
	AVXLemmaOOV { key: 0x873C, word: "forfeit" },
	AVXLemmaOOV { key: 0x873D, word: "freckle" },
	AVXLemmaOOV { key: 0x873E, word: "furlong" },
	AVXLemmaOOV { key: 0x873F, word: "garland" },
	AVXLemmaOOV { key: 0x8740, word: "gezrite" },
	AVXLemmaOOV { key: 0x8741, word: "glister" },
	AVXLemmaOOV { key: 0x8742, word: "glorifi" },
	AVXLemmaOOV { key: 0x8743, word: "grafted" },
	AVXLemmaOOV { key: 0x8744, word: "grecian" },
	AVXLemmaOOV { key: 0x8745, word: "grinder" },
	AVXLemmaOOV { key: 0x8746, word: "grizzle" },
	AVXLemmaOOV { key: 0x8747, word: "haggite" },
	AVXLemmaOOV { key: 0x8748, word: "heretic" },
	AVXLemmaOOV { key: 0x8749, word: "hostage" },
	AVXLemmaOOV { key: 0x874A, word: "inflict" },
	AVXLemmaOOV { key: 0x874B, word: "inquire" },
	AVXLemmaOOV { key: 0x874C, word: "intrude" },
	AVXLemmaOOV { key: 0x874D, word: "jeopard" },
	AVXLemmaOOV { key: 0x874E, word: "jimnite" },
	AVXLemmaOOV { key: 0x874F, word: "justifi" },
	AVXLemmaOOV { key: 0x8750, word: "korhite" },
	AVXLemmaOOV { key: 0x8751, word: "lantern" },
	AVXLemmaOOV { key: 0x8752, word: "libnite" },
	AVXLemmaOOV { key: 0x8753, word: "lunatic" },
	AVXLemmaOOV { key: 0x8754, word: "mahlite" },
	AVXLemmaOOV { key: 0x8755, word: "mansion" },
	AVXLemmaOOV { key: 0x8756, word: "maonite" },
	AVXLemmaOOV { key: 0x8757, word: "mariner" },
	AVXLemmaOOV { key: 0x8758, word: "million" },
	AVXLemmaOOV { key: 0x8759, word: "moisten" },
	AVXLemmaOOV { key: 0x875A, word: "mollify" },
	AVXLemmaOOV { key: 0x875B, word: "monster" },
	AVXLemmaOOV { key: 0x875C, word: "naamite" },
	AVXLemmaOOV { key: 0x875D, word: "neesing" },
	AVXLemmaOOV { key: 0x875E, word: "nostril" },
	AVXLemmaOOV { key: 0x875F, word: "outlive" },
	AVXLemmaOOV { key: 0x8760, word: "overrun" },
	AVXLemmaOOV { key: 0x8761, word: "parmena" },
	AVXLemmaOOV { key: 0x8762, word: "peacock" },
	AVXLemmaOOV { key: 0x8763, word: "perjure" },
	AVXLemmaOOV { key: 0x8764, word: "perplex" },
	AVXLemmaOOV { key: 0x8765, word: "picture" },
	AVXLemmaOOV { key: 0x8766, word: "pilgrim" },
	AVXLemmaOOV { key: 0x8767, word: "planter" },
	AVXLemmaOOV { key: 0x8768, word: "plaster" },
	AVXLemmaOOV { key: 0x8769, word: "portray" },
	AVXLemmaOOV { key: 0x876A, word: "purloin" },
	AVXLemmaOOV { key: 0x876B, word: "putrify" },
	AVXLemmaOOV { key: 0x876C, word: "resolve" },
	AVXLemmaOOV { key: 0x876D, word: "reviler" },
	AVXLemmaOOV { key: 0x876E, word: "saltpit" },
	AVXLemmaOOV { key: 0x876F, word: "sardite" },
	AVXLemmaOOV { key: 0x8770, word: "satisfi" },
	AVXLemmaOOV { key: 0x8771, word: "scoffer" },
	AVXLemmaOOV { key: 0x8772, word: "seducer" },
	AVXLemmaOOV { key: 0x8773, word: "senator" },
	AVXLemmaOOV { key: 0x8774, word: "shausha" },
	AVXLemmaOOV { key: 0x8775, word: "sheriff" },
	AVXLemmaOOV { key: 0x8776, word: "shimite" },
	AVXLemmaOOV { key: 0x8777, word: "shipman" },
	AVXLemmaOOV { key: 0x8778, word: "shooter" },
	AVXLemmaOOV { key: 0x8779, word: "shorten" },
	AVXLemmaOOV { key: 0x877A, word: "slinger" },
	AVXLemmaOOV { key: 0x877B, word: "snuffer" },
	AVXLemmaOOV { key: 0x877C, word: "sparkle" },
	AVXLemmaOOV { key: 0x877D, word: "speckle" },
	AVXLemmaOOV { key: 0x877E, word: "stammer" },
	AVXLemmaOOV { key: 0x877F, word: "staunch" },
	AVXLemmaOOV { key: 0x8780, word: "stiffen" },
	AVXLemmaOOV { key: 0x8781, word: "sukkiim" },
	AVXLemmaOOV { key: 0x8782, word: "surfeit" },
	AVXLemmaOOV { key: 0x8783, word: "swaddle" },
	AVXLemmaOOV { key: 0x8784, word: "swearer" },
	AVXLemmaOOV { key: 0x8785, word: "tanhume" },
	AVXLemmaOOV { key: 0x8786, word: "tattler" },
	AVXLemmaOOV { key: 0x8787, word: "terrace" },
	AVXLemmaOOV { key: 0x8788, word: "terrifi" },
	AVXLemmaOOV { key: 0x8789, word: "torture" },
	AVXLemmaOOV { key: 0x878A, word: "tossing" },
	AVXLemmaOOV { key: 0x878B, word: "traffic" },
	AVXLemmaOOV { key: 0x878C, word: "trickle" },
	AVXLemmaOOV { key: 0x878D, word: "twinkle" },
	AVXLemmaOOV { key: 0x878E, word: "unaware" },
	AVXLemmaOOV { key: 0x878F, word: "undress" },
	AVXLemmaOOV { key: 0x8790, word: "unladen" },
	AVXLemmaOOV { key: 0x8791, word: "violate" },
	AVXLemmaOOV { key: 0x8792, word: "wayfare" },
	AVXLemmaOOV { key: 0x8793, word: "waymark" },
	AVXLemmaOOV { key: 0x8794, word: "witting" },
	AVXLemmaOOV { key: 0x8795, word: "wreathe" },
	AVXLemmaOOV { key: 0x8796, word: "zarhite" },
	AVXLemmaOOV { key: 0x8801, word: "aaronite" },
	AVXLemmaOOV { key: 0x8802, word: "accustom" },
	AVXLemmaOOV { key: 0x8803, word: "almsdeed" },
	AVXLemmaOOV { key: 0x8804, word: "amramite" },
	AVXLemmaOOV { key: 0x8805, word: "ancestor" },
	AVXLemmaOOV { key: 0x8806, word: "apparent" },
	AVXLemmaOOV { key: 0x8807, word: "argument" },
	AVXLemmaOOV { key: 0x8808, word: "asherite" },
	AVXLemmaOOV { key: 0x8809, word: "ashurite" },
	AVXLemmaOOV { key: 0x880A, word: "astonish" },
	AVXLemmaOOV { key: 0x880B, word: "athenian" },
	AVXLemmaOOV { key: 0x880C, word: "axletree" },
	AVXLemmaOOV { key: 0x880D, word: "bachrite" },
	AVXLemmaOOV { key: 0x880E, word: "backbite" },
	AVXLemmaOOV { key: 0x880F, word: "beatrice" },
	AVXLemmaOOV { key: 0x8810, word: "believer" },
	AVXLemmaOOV { key: 0x8811, word: "betrayer" },
	AVXLemmaOOV { key: 0x8812, word: "betrothe" },
	AVXLemmaOOV { key: 0x8813, word: "ceremony" },
	AVXLemmaOOV { key: 0x8814, word: "chemarim" },
	AVXLemmaOOV { key: 0x8815, word: "cherubin" },
	AVXLemmaOOV { key: 0x8816, word: "cockcrow" },
	AVXLemmaOOV { key: 0x8817, word: "conspire" },
	AVXLemmaOOV { key: 0x8818, word: "cracknel" },
	AVXLemmaOOV { key: 0x8819, word: "cucumber" },
	AVXLemmaOOV { key: 0x881A, word: "dehavite" },
	AVXLemmaOOV { key: 0x881B, word: "delicacy" },
	AVXLemmaOOV { key: 0x881C, word: "despiser" },
	AVXLemmaOOV { key: 0x881D, word: "devotion" },
	AVXLemmaOOV { key: 0x881E, word: "distinct" },
	AVXLemmaOOV { key: 0x881F, word: "distract" },
	AVXLemmaOOV { key: 0x8820, word: "ekronite" },
	AVXLemmaOOV { key: 0x8821, word: "embolden" },
	AVXLemmaOOV { key: 0x8822, word: "especial" },
	AVXLemmaOOV { key: 0x8823, word: "espousal" },
	AVXLemmaOOV { key: 0x8824, word: "estrange" },
	AVXLemmaOOV { key: 0x8825, word: "exorcist" },
	AVXLemmaOOV { key: 0x8826, word: "fishhook" },
	AVXLemmaOOV { key: 0x8827, word: "fishpool" },
	AVXLemmaOOV { key: 0x8828, word: "follower" },
	AVXLemmaOOV { key: 0x8829, word: "footstep" },
	AVXLemmaOOV { key: 0x882A, word: "fragment" },
	AVXLemmaOOV { key: 0x882B, word: "frontier" },
	AVXLemmaOOV { key: 0x882C, word: "frontlet" },
	AVXLemmaOOV { key: 0x882D, word: "gammadim" },
	AVXLemmaOOV { key: 0x882E, word: "gathheph" },
	AVXLemmaOOV { key: 0x882F, word: "goatskin" },
	AVXLemmaOOV { key: 0x8830, word: "hagarite" },
	AVXLemmaOOV { key: 0x8831, word: "headband" },
	AVXLemmaOOV { key: 0x8832, word: "heberite" },
	AVXLemmaOOV { key: 0x8833, word: "helekite" },
	AVXLemmaOOV { key: 0x8834, word: "herdsman" },
	AVXLemmaOOV { key: 0x8835, word: "imprison" },
	AVXLemmaOOV { key: 0x8836, word: "ingather" },
	AVXLemmaOOV { key: 0x8837, word: "inventor" },
	AVXLemmaOOV { key: 0x8838, word: "jaminite" },
	AVXLemmaOOV { key: 0x8839, word: "jezerite" },
	AVXLemmaOOV { key: 0x883A, word: "kerchief" },
	AVXLemmaOOV { key: 0x883B, word: "mandrake" },
	AVXLemmaOOV { key: 0x883C, word: "merarite" },
	AVXLemmaOOV { key: 0x883D, word: "miscarry" },
	AVXLemmaOOV { key: 0x883E, word: "moderate" },
	AVXLemmaOOV { key: 0x883F, word: "monument" },
	AVXLemmaOOV { key: 0x8840, word: "mortgage" },
	AVXLemmaOOV { key: 0x8841, word: "mournful" },
	AVXLemmaOOV { key: 0x8842, word: "murmurer" },
	AVXLemmaOOV { key: 0x8843, word: "nethinim" },
	AVXLemmaOOV { key: 0x8844, word: "occupier" },
	AVXLemmaOOV { key: 0x8845, word: "ordering" },
	AVXLemmaOOV { key: 0x8846, word: "outgoing" },
	AVXLemmaOOV { key: 0x8847, word: "overlive" },
	AVXLemmaOOV { key: 0x8848, word: "palluite" },
	AVXLemmaOOV { key: 0x8849, word: "paramour" },
	AVXLemmaOOV { key: 0x884A, word: "parthian" },
	AVXLemmaOOV { key: 0x884B, word: "pedigree" },
	AVXLemmaOOV { key: 0x884C, word: "practice" },
	AVXLemmaOOV { key: 0x884D, word: "renounce" },
	AVXLemmaOOV { key: 0x884E, word: "revolter" },
	AVXLemmaOOV { key: 0x884F, word: "rudiment" },
	AVXLemmaOOV { key: 0x8850, word: "sadducee" },
	AVXLemmaOOV { key: 0x8851, word: "sanctifi" },
	AVXLemmaOOV { key: 0x8852, word: "scrabble" },
	AVXLemmaOOV { key: 0x8853, word: "seraphim" },
	AVXLemmaOOV { key: 0x8854, word: "shallech" },
	AVXLemmaOOV { key: 0x8855, word: "shibbole" },
	AVXLemmaOOV { key: 0x8856, word: "slimepit" },
	AVXLemmaOOV { key: 0x8857, word: "soothsay" },
	AVXLemmaOOV { key: 0x8858, word: "spearman" },
	AVXLemmaOOV { key: 0x8859, word: "spiteful" },
	AVXLemmaOOV { key: 0x885A, word: "strangle" },
	AVXLemmaOOV { key: 0x885B, word: "struggle" },
	AVXLemmaOOV { key: 0x885C, word: "surprise" },
	AVXLemmaOOV { key: 0x885D, word: "tahanite" },
	AVXLemmaOOV { key: 0x885E, word: "tahapane" },
	AVXLemmaOOV { key: 0x885F, word: "tahpanhe" },
	AVXLemmaOOV { key: 0x8860, word: "transfer" },
	AVXLemmaOOV { key: 0x8861, word: "traverse" },
	AVXLemmaOOV { key: 0x8862, word: "unclothe" },
	AVXLemmaOOV { key: 0x8863, word: "unoccupy" },
	AVXLemmaOOV { key: 0x8864, word: "vestment" },
	AVXLemmaOOV { key: 0x8865, word: "wanderer" },
	AVXLemmaOOV { key: 0x8866, word: "wrongful" },
	AVXLemmaOOV { key: 0x8901, word: "ahiramite" },
	AVXLemmaOOV { key: 0x8902, word: "archevite" },
	AVXLemmaOOV { key: 0x8903, word: "ashbelite" },
	AVXLemmaOOV { key: 0x8904, word: "ashdodite" },
	AVXLemmaOOV { key: 0x8905, word: "asrielite" },
	AVXLemmaOOV { key: 0x8906, word: "backbiter" },
	AVXLemmaOOV { key: 0x8907, word: "backslide" },
	AVXLemmaOOV { key: 0x8908, word: "balancing" },
	AVXLemmaOOV { key: 0x8909, word: "besteaded" },
	AVXLemmaOOV { key: 0x890A, word: "bishopric" },
	AVXLemmaOOV { key: 0x890B, word: "blindfold" },
	AVXLemmaOOV { key: 0x890C, word: "chaldaean" },
	AVXLemmaOOV { key: 0x890D, word: "challenge" },
	AVXLemmaOOV { key: 0x890E, word: "cherethim" },
	AVXLemmaOOV { key: 0x890F, word: "childbear" },
	AVXLemmaOOV { key: 0x8910, word: "chronicle" },
	AVXLemmaOOV { key: 0x8911, word: "conqueror" },
	AVXLemmaOOV { key: 0x8912, word: "corrupter" },
	AVXLemmaOOV { key: 0x8913, word: "damascene" },
	AVXLemmaOOV { key: 0x8914, word: "delicious" },
	AVXLemmaOOV { key: 0x8915, word: "discomfit" },
	AVXLemmaOOV { key: 0x8916, word: "dissemble" },
	AVXLemmaOOV { key: 0x8917, word: "diversity" },
	AVXLemmaOOV { key: 0x8918, word: "enclosing" },
	AVXLemmaOOV { key: 0x8919, word: "encounter" },
	AVXLemmaOOV { key: 0x891A, word: "engraving" },
	AVXLemmaOOV { key: 0x891B, word: "exchanger" },
	AVXLemmaOOV { key: 0x891C, word: "eziongabe" },
	AVXLemmaOOV { key: 0x891D, word: "eziongebe" },
	AVXLemmaOOV { key: 0x891E, word: "fisherman" },
	AVXLemmaOOV { key: 0x891F, word: "gainsayer" },
	AVXLemmaOOV { key: 0x8920, word: "gazathite" },
	AVXLemmaOOV { key: 0x8921, word: "geshurite" },
	AVXLemmaOOV { key: 0x8922, word: "hailstone" },
	AVXLemmaOOV { key: 0x8923, word: "hammoleke" },
	AVXLemmaOOV { key: 0x8924, word: "handstave" },
	AVXLemmaOOV { key: 0x8925, word: "hanochite" },
	AVXLemmaOOV { key: 0x8926, word: "hebronite" },
	AVXLemmaOOV { key: 0x8927, word: "hepherite" },
	AVXLemmaOOV { key: 0x8928, word: "hermonite" },
	AVXLemmaOOV { key: 0x8929, word: "hezronite" },
	AVXLemmaOOV { key: 0x892A, word: "huphamite" },
	AVXLemmaOOV { key: 0x892B, word: "immediate" },
	AVXLemmaOOV { key: 0x892C, word: "influence" },
	AVXLemmaOOV { key: 0x892D, word: "inhabiter" },
	AVXLemmaOOV { key: 0x892E, word: "invention" },
	AVXLemmaOOV { key: 0x892F, word: "jachinite" },
	AVXLemmaOOV { key: 0x8930, word: "jashubite" },
	AVXLemmaOOV { key: 0x8931, word: "jeezerite" },
	AVXLemmaOOV { key: 0x8932, word: "jerubbesh" },
	AVXLemmaOOV { key: 0x8933, word: "jointheir" },
	AVXLemmaOOV { key: 0x8934, word: "kohathite" },
	AVXLemmaOOV { key: 0x8935, word: "korathite" },
	AVXLemmaOOV { key: 0x8936, word: "libertine" },
	AVXLemmaOOV { key: 0x8937, word: "machirite" },
	AVXLemmaOOV { key: 0x8938, word: "mishraite" },
	AVXLemmaOOV { key: 0x8939, word: "nemuelite" },
	AVXLemmaOOV { key: 0x893A, word: "parchment" },
	AVXLemmaOOV { key: 0x893B, word: "passenger" },
	AVXLemmaOOV { key: 0x893C, word: "prescribe" },
	AVXLemmaOOV { key: 0x893D, word: "president" },
	AVXLemmaOOV { key: 0x893E, word: "principle" },
	AVXLemmaOOV { key: 0x893F, word: "quicksand" },
	AVXLemmaOOV { key: 0x8940, word: "rechabite" },
	AVXLemmaOOV { key: 0x8941, word: "recommend" },
	AVXLemmaOOV { key: 0x8942, word: "secondary" },
	AVXLemmaOOV { key: 0x8943, word: "shameless" },
	AVXLemmaOOV { key: 0x8944, word: "sheepskin" },
	AVXLemmaOOV { key: 0x8945, word: "shelanite" },
	AVXLemmaOOV { key: 0x8946, word: "shuhamite" },
	AVXLemmaOOV { key: 0x8947, word: "slanderer" },
	AVXLemmaOOV { key: 0x8948, word: "snuffdish" },
	AVXLemmaOOV { key: 0x8949, word: "stammerer" },
	AVXLemmaOOV { key: 0x894A, word: "stargazer" },
	AVXLemmaOOV { key: 0x894B, word: "steadfast" },
	AVXLemmaOOV { key: 0x894C, word: "sumptuous" },
	AVXLemmaOOV { key: 0x894D, word: "suppliant" },
	AVXLemmaOOV { key: 0x894E, word: "tarpelite" },
	AVXLemmaOOV { key: 0x894F, word: "tentmaker" },
	AVXLemmaOOV { key: 0x8950, word: "tirathite" },
	AVXLemmaOOV { key: 0x8951, word: "tormentor" },
	AVXLemmaOOV { key: 0x8952, word: "transform" },
	AVXLemmaOOV { key: 0x8953, word: "trumpeter" },
	AVXLemmaOOV { key: 0x8954, word: "unadvised" },
	AVXLemmaOOV { key: 0x8955, word: "unbelieve" },
	AVXLemmaOOV { key: 0x8956, word: "undergird" },
	AVXLemmaOOV { key: 0x8957, word: "unwitting" },
	AVXLemmaOOV { key: 0x8958, word: "uzzielite" },
	AVXLemmaOOV { key: 0x8959, word: "zephonite" },
	AVXLemmaOOV { key: 0x895A, word: "zorathite" },
	AVXLemmaOOV { key: 0x8A01, word: "administer" },
	AVXLemmaOOV { key: 0x8A02, word: "ashdothite" },
	AVXLemmaOOV { key: 0x8A03, word: "babylonian" },
	AVXLemmaOOV { key: 0x8A04, word: "benefactor" },
	AVXLemmaOOV { key: 0x8A05, word: "chalkstone" },
	AVXLemmaOOV { key: 0x8A06, word: "charitable" },
	AVXLemmaOOV { key: 0x8A07, word: "cogitation" },
	AVXLemmaOOV { key: 0x8A08, word: "complainer" },
	AVXLemmaOOV { key: 0x8A09, word: "contradict" },
	AVXLemmaOOV { key: 0x8A0A, word: "corinthian" },
	AVXLemmaOOV { key: 0x8A0B, word: "countryman" },
	AVXLemmaOOV { key: 0x8A0C, word: "discontent" },
	AVXLemmaOOV { key: 0x8A0D, word: "dispersion" },
	AVXLemmaOOV { key: 0x8A0E, word: "dissembler" },
	AVXLemmaOOV { key: 0x8A0F, word: "eyewitness" },
	AVXLemmaOOV { key: 0x8A10, word: "fellowheir" },
	AVXLemmaOOV { key: 0x8A11, word: "forefather" },
	AVXLemmaOOV { key: 0x8A12, word: "foreordain" },
	AVXLemmaOOV { key: 0x8A13, word: "gittahheph" },
	AVXLemmaOOV { key: 0x8A14, word: "illuminate" },
	AVXLemmaOOV { key: 0x8A15, word: "jahleelite" },
	AVXLemmaOOV { key: 0x8A16, word: "jahzeelite" },
	AVXLemmaOOV { key: 0x8A17, word: "lieutenant" },
	AVXLemmaOOV { key: 0x8A18, word: "longsuffer" },
	AVXLemmaOOV { key: 0x8A19, word: "menpleaser" },
	AVXLemmaOOV { key: 0x8A1A, word: "menstealer" },
	AVXLemmaOOV { key: 0x8A1B, word: "meshulleme" },
	AVXLemmaOOV { key: 0x8A1C, word: "opposition" },
	AVXLemmaOOV { key: 0x8A1D, word: "outstretch" },
	AVXLemmaOOV { key: 0x8A1E, word: "peacemaker" },
	AVXLemmaOOV { key: 0x8A1F, word: "philippian" },
	AVXLemmaOOV { key: 0x8A20, word: "phylactery" },
	AVXLemmaOOV { key: 0x8A21, word: "progenitor" },
	AVXLemmaOOV { key: 0x8A22, word: "quaternion" },
	AVXLemmaOOV { key: 0x8A23, word: "repetition" },
	AVXLemmaOOV { key: 0x8A24, word: "romamtieze" },
	AVXLemmaOOV { key: 0x8A25, word: "sepharvite" },
	AVXLemmaOOV { key: 0x8A26, word: "shechemite" },
	AVXLemmaOOV { key: 0x8A27, word: "shemidaite" },
	AVXLemmaOOV { key: 0x8A28, word: "shillemite" },
	AVXLemmaOOV { key: 0x8A29, word: "shimronite" },
	AVXLemmaOOV { key: 0x8A2A, word: "shumathite" },
	AVXLemmaOOV { key: 0x8A2B, word: "shuphamite" },
	AVXLemmaOOV { key: 0x8A2C, word: "silverling" },
	AVXLemmaOOV { key: 0x8A2D, word: "slanderous" },
	AVXLemmaOOV { key: 0x8A2E, word: "slingstone" },
	AVXLemmaOOV { key: 0x8A2F, word: "slumbering" },
	AVXLemmaOOV { key: 0x8A30, word: "susanchite" },
	AVXLemmaOOV { key: 0x8A31, word: "taskmaster" },
	AVXLemmaOOV { key: 0x8A32, word: "tehaphnehe" },
	AVXLemmaOOV { key: 0x8A33, word: "thundering" },
	AVXLemmaOOV { key: 0x8A34, word: "trafficker" },
	AVXLemmaOOV { key: 0x8A35, word: "unbeliever" },
	AVXLemmaOOV { key: 0x8A36, word: "unblamable" },
	AVXLemmaOOV { key: 0x8A37, word: "waterspout" },
	AVXLemmaOOV { key: 0x8A38, word: "zareathite" },
	AVXLemmaOOV { key: 0x8B01, word: "alexandrian" },
	AVXLemmaOOV { key: 0x8B02, word: "conspirator" },
	AVXLemmaOOV { key: 0x8B03, word: "eshkalonite" },
	AVXLemmaOOV { key: 0x8B04, word: "fellowsoldy" },
	AVXLemmaOOV { key: 0x8B05, word: "kirjathseph" },
	AVXLemmaOOV { key: 0x8B06, word: "malchielite" },
	AVXLemmaOOV { key: 0x8B07, word: "merchantman" },
	AVXLemmaOOV { key: 0x8B08, word: "philosopher" },
	AVXLemmaOOV { key: 0x8B09, word: "pruninghook" },
	AVXLemmaOOV { key: 0x8B0A, word: "reproachful" },
	AVXLemmaOOV { key: 0x8B0B, word: "shimeathite" },
	AVXLemmaOOV { key: 0x8B0C, word: "shuthalhite" },
	AVXLemmaOOV { key: 0x8B0D, word: "thunderbolt" },
	AVXLemmaOOV { key: 0x8B0E, word: "transfigure" },
	AVXLemmaOOV { key: 0x8B0F, word: "undersetter" },
	AVXLemmaOOV { key: 0x8B10, word: "vinedresser" },
	AVXLemmaOOV { key: 0x8B11, word: "waterspring" },
	AVXLemmaOOV { key: 0x8C01, word: "affectionate" },
	AVXLemmaOOV { key: 0x8C02, word: "apharsachite" },
	AVXLemmaOOV { key: 0x8C03, word: "contemptuous" },
	AVXLemmaOOV { key: 0x8C04, word: "fellowworker" },
	AVXLemmaOOV { key: 0x8C05, word: "handkerchief" },
	AVXLemmaOOV { key: 0x8C06, word: "jerahmeelite" },
	AVXLemmaOOV { key: 0x8C07, word: "lookingglass" },
	AVXLemmaOOV { key: 0x8C08, word: "moneychanger" },
	AVXLemmaOOV { key: 0x8C09, word: "sheepshearer" },
	AVXLemmaOOV { key: 0x8C0A, word: "stonesquarer" },
	AVXLemmaOOV { key: 0x8C0B, word: "trucebreaker" },
	AVXLemmaOOV { key: 0x8C0C, word: "womenservant" },
	AVXLemmaOOV { key: 0x8D01, word: "confectionary" },
	AVXLemmaOOV { key: 0x8D02, word: "constellation" },
	AVXLemmaOOV { key: 0x8D03, word: "fellowcitizen" },
	AVXLemmaOOV { key: 0x8D04, word: "grapegleaning" },
	AVXLemmaOOV { key: 0x8D05, word: "nergalshareze" },
	AVXLemmaOOV { key: 0x8D06, word: "shoulderpiece" },
	AVXLemmaOOV { key: 0x8D07, word: "tiglathpilese" },
	AVXLemmaOOV { key: 0x8E01, word: "apharsathchite" },
	AVXLemmaOOV { key: 0x8E02, word: "fellowdisciple" },
	AVXLemmaOOV { key: 0x8E03, word: "kneadingtrough" },
	AVXLemmaOOV { key: 0x8E04, word: "prognosticator" },
	AVXLemmaOOV { key: 0x8E05, word: "tilgathpilnese" },
	AVXLemmaOOV { key: 0x8F01, word: "covenantbreaker" },
];
// < < < Generated-Code -- Initialization < < < //
