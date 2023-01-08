
pub mod avx;

use generated::book_index::avx::flat_buf;
use crate::generated::book_index::avx::flat_buf::{AVXBook, AVXBookIndex};

use std::fs::File;
use std::io::Read;

pub fn deserialize_avxbook_index() -> Result<Vec<AVXBook>, flatbuffers::InvalidFlatbuffer> {

    let mut idx_buf : Vec<u8> = Vec::with_capacity(999);
    for _ in (0..idx_buf.capacity()) {
        idx_buf.push(0);
    }
    let mut file = match File::open("foo.avx") {
        Ok(file) => file,
        Err(_) => panic!("no such file"),
    };
    file.read(&mut idx_buf[..]).unwrap();
    let book_index = generated::book_index::avx::flat_buf::AVXBookIndex::size_prefixed_root_as_avxbook_index(idx_buf);
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn valid_book_index() {


        let result = add(2, 2);
        assert_eq!(result, 4);
    }
}
