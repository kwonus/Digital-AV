
pub mod avx;

//use std::fs::File;
//use std::io::Read;

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn valid_book_index() {
        let result = add(2, 2);
        assert_eq!(result, 4);
    }
}