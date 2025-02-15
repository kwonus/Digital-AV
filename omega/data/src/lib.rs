
#[no_mangle]
pub extern "C" fn acquire_data(data: *mut u8, size: usize) -> usize {
    let tuple = include_bytes!("AVX-Omega-5104.data");

    if size == 0 {
        return tuple.len();
    }
    unsafe {
        if size >= tuple.len() {
            std::ptr::copy_nonoverlapping(tuple.as_ptr(), data, tuple.len());
        }
        else {
            return 0;
        }
    }
    tuple.len()
}

#[no_mangle]
pub unsafe extern "C" fn md5_hash(idx: u8) -> u64 {
    match idx {
        0 => 0x30BC363A7AB86D4B,
        1 => 0x312074329710CF42,
        _ => 0,
    }
}

#[no_mangle]
pub unsafe extern "C" fn get_library_revision() -> u16 {
    return 0x5104;
}