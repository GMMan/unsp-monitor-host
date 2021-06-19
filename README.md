μ'nSP Monitor host application
==============================

This program is the host application for interacting with the [μ'nSP monitor](https://github.com/GMMan/unsp-monitor).

Usage
-----

```
UnspMon <port> <operation> <address> [value]
```

- `port`: the serial port your target is connected on, e.g. `COM3`
- `operation`: one of several command names
- `address`: the address to run the command on
- `value`: optional argument for command

Some commands may take input from standard input and write output to standard
output.

Commands
--------

### read

Reads a word or many words from target.

With argument: reads `value` words from address and writes it as binary to
standard output.

Without argument: reads single word from address and writes prints it as hex.

### write

Writes a word or many words to target.

With argument: writes the word `value` to the address.

Without argument: writes data from standard input to the address. Limited to
`0xffff` bytes.

### call

Calls the code at the address.

### exec

Uploads code from standard input to the address and calls it.
